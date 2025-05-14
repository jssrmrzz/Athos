using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Athos.ReviewAutomation.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace Athos.ReviewAutomation.Infrastructure.LLM
{
    public class LocalLlmClient : ILlmClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public LocalLlmClient(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient("LocalLLM");
            _config = config;
        }

        public async Task<string> GenerateResponseAsync(string reviewText)
        {
            var requestBody = new
            {
                model = _config["LLM:Local:Model"] ?? "llama3",
                messages = new[]
                {
                    new { role = "system", content = "You're a polite assistant that writes short, helpful replies to customer reviews." },
                    new { role = "user", content = reviewText }
                },
                temperature = 0.7
            };

            var response = await _httpClient.PostAsJsonAsync("v1/chat/completions", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"❌ Local LLM error: {response.StatusCode} - {error}");
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var json = await JsonDocument.ParseAsync(stream);

            if (!json.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
            {
                throw new ApplicationException("❌ Local LLM returned no valid choices.");
            }

            var message = choices[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return message?.Trim() ?? "(No response generated)";
        }
    }
}

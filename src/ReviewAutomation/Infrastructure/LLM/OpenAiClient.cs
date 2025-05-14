using Athos.ReviewAutomation.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Athos.ReviewAutomation.Infrastructure.Services
{
    public class OpenAiClient : ILlmClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;


        public OpenAiClient(IHttpClientFactory factory, IConfiguration config)

        {
            _httpClient = factory.CreateClient("OpenAI");
            _apiKey = config["LLM:OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key not configured.");

        }

        public async Task<string> GenerateResponseAsync(string reviewText)
        {
            var request = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "Write a short, polite response to a customer review." },
                    new { role = "user", content = reviewText }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"OpenAI API call failed: {response.StatusCode}");

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(responseStream);
            return document.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
        }
    }
}
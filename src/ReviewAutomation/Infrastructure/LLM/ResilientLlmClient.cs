using Athos.ReviewAutomation.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Athos.ReviewAutomation.Infrastructure.LLM
{
    public class ResilientLlmClient : ILlmClient
    {
        private readonly ILlmClient _inner;
        private readonly ILogger<ResilientLlmClient> _logger;
        private const int MaxRetries = 3;

        public ResilientLlmClient(ILlmClient inner, ILogger<ResilientLlmClient> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<string> GenerateResponseAsync(string reviewText)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    attempt++;
                    return await _inner.GenerateResponseAsync(reviewText);
                }
                catch (Exception ex) when (attempt <= MaxRetries)
                {
                    _logger.LogWarning($"Attempt {attempt} failed: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(1.5 * attempt));
                }
                catch (Exception finalEx)
                {
                    _logger.LogError(finalEx, "All retries failed. Returning fallback message.");
                    return "Sorry, we're currently unable to generate a reply. Please try again later.";
                }
            }
        }
    }
}
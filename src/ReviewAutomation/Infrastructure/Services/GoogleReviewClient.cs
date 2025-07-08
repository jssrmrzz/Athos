using System.Net.Http.Json;
using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Models;
using Microsoft.Extensions.Logging;

namespace Athos.ReviewAutomation.Infrastructure.Services
{
    public class GoogleReviewClient
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticatedGoogleApiClient _authenticatedClient;
        private readonly ILogger<GoogleReviewClient> _logger;

        public GoogleReviewClient(
            HttpClient httpClient,
            AuthenticatedGoogleApiClient authenticatedClient,
            ILogger<GoogleReviewClient> logger)
        {
            _httpClient = httpClient;
            _authenticatedClient = authenticatedClient;
            _logger = logger;
            Console.WriteLine($"✅ GoogleReviewClient base URL: {_httpClient.BaseAddress}");
        }

        public async Task<List<GoogleReviewDto>> FetchReviewsAsync()
        {
            // Fallback to mock API
            var response = await _httpClient.GetFromJsonAsync<List<GoogleReviewDto>>("mockgoogleapi/mock-reviews");
            return response ?? new List<GoogleReviewDto>();
        }

        public async Task<List<GoogleReviewDto>> FetchReviewsForBusinessAsync(int businessId, string? googleBusinessProfileId = null)
        {
            try
            {
                // Try to use authenticated API first
                return await _authenticatedClient.FetchBusinessReviewsAsync(businessId, googleBusinessProfileId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch reviews using OAuth for business {BusinessId}, falling back to mock API", businessId);
                
                // Fallback to mock API
                return await FetchReviewsAsync();
            }
        }


        private int StarRatingToInt(string starRating)
        {
            return starRating?.ToUpperInvariant() switch
            {
                "ONE" => 1,
                "TWO" => 2,
                "THREE" => 3,
                "FOUR" => 4,
                "FIVE" => 5,
                _ => 0
            };

        }
    }
}
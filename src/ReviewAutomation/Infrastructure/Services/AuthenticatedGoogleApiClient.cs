using System.Net.Http.Headers;
using System.Text.Json;
using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Models;
using Microsoft.Extensions.Logging;

namespace Athos.ReviewAutomation.Infrastructure.Services
{
    public class AuthenticatedGoogleApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IGoogleOAuthService _oauthService;
        private readonly ILogger<AuthenticatedGoogleApiClient> _logger;

        public AuthenticatedGoogleApiClient(
            IHttpClientFactory httpClientFactory,
            IGoogleOAuthService oauthService,
            ILogger<AuthenticatedGoogleApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _oauthService = oauthService;
            _logger = logger;
        }

        public async Task<List<GoogleReviewDto>> FetchBusinessReviewsAsync(int businessId, string? googleBusinessProfileId = null)
        {
            var accessToken = await _oauthService.GetValidAccessTokenAsync(businessId);
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new InvalidOperationException($"No valid access token available for business {businessId}");
            }

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                // First, get the business accounts if we don't have a profile ID
                if (string.IsNullOrEmpty(googleBusinessProfileId))
                {
                    var accounts = await GetBusinessAccountsAsync(httpClient);
                    if (accounts.Count == 0)
                    {
                        _logger.LogWarning("No business accounts found for business {BusinessId}", businessId);
                        return new List<GoogleReviewDto>();
                    }
                    googleBusinessProfileId = accounts[0].Name; // Use first account for now
                }

                // Get reviews for the business
                var reviews = await GetReviewsForLocationAsync(httpClient, googleBusinessProfileId);
                return reviews;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch reviews for business {BusinessId}", businessId);
                throw;
            }
        }

        private async Task<List<GoogleBusinessAccount>> GetBusinessAccountsAsync(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync("https://mybusinessaccountmanagement.googleapis.com/v1/accounts");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var accountsResponse = JsonSerializer.Deserialize<GoogleAccountsResponse>(content);
            
            return accountsResponse?.Accounts ?? new List<GoogleBusinessAccount>();
        }

        private async Task<List<GoogleReviewDto>> GetReviewsForLocationAsync(HttpClient httpClient, string accountName)
        {
            // Get locations for the account
            var locationsResponse = await httpClient.GetAsync($"https://mybusinessbusinessinformation.googleapis.com/v1/{accountName}/locations");
            locationsResponse.EnsureSuccessStatusCode();
            
            var locationsContent = await locationsResponse.Content.ReadAsStringAsync();
            var locations = JsonSerializer.Deserialize<GoogleLocationsResponse>(locationsContent);
            
            var allReviews = new List<GoogleReviewDto>();
            
            if (locations?.Locations != null)
            {
                foreach (var location in locations.Locations)
                {
                    try
                    {
                        var reviewsResponse = await httpClient.GetAsync($"https://mybusiness.googleapis.com/v4/{location.Name}/reviews");
                        if (reviewsResponse.IsSuccessStatusCode)
                        {
                            var reviewsContent = await reviewsResponse.Content.ReadAsStringAsync();
                            var reviewsData = JsonSerializer.Deserialize<GoogleReviewsResponse>(reviewsContent);
                            
                            if (reviewsData?.Reviews != null)
                            {
                                allReviews.AddRange(reviewsData.Reviews);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to fetch reviews for location {LocationName}", location.Name);
                    }
                }
            }
            
            return allReviews;
        }

        private class GoogleAccountsResponse
        {
            public List<GoogleBusinessAccount> Accounts { get; set; } = new();
        }

        private class GoogleBusinessAccount
        {
            public string Name { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
        }

        private class GoogleLocationsResponse
        {
            public List<GoogleBusinessLocation> Locations { get; set; } = new();
        }

        private class GoogleBusinessLocation
        {
            public string Name { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
        }

        private class GoogleReviewsResponse
        {
            public List<GoogleReviewDto> Reviews { get; set; } = new();
        }
    }
}
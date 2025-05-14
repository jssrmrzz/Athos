using System.Net.Http.Json;
using Athos.ReviewAutomation.Models;

namespace Athos.ReviewAutomation.Infrastructure.Services
{
    public class GoogleReviewClient
    {
        private readonly HttpClient _httpClient;

        public GoogleReviewClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Console.WriteLine($"✅ GoogleReviewClient base URL: {_httpClient.BaseAddress}");

        }

        public async Task<List<GoogleReviewDto>> FetchReviewsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<GoogleReviewDto>>("mockgoogleapi/mock-reviews");
            return response ?? new List<GoogleReviewDto>();
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
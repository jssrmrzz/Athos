using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading;

namespace Athos.ReviewAutomation.Api.Controllers
{
    [ApiController]
    [Route("api/mock")]
    public class MockReviewApiController : ControllerBase
    {
        // 🧠 In-memory store for mock reviews
        private static readonly List<MockReview> MockReviews = new()
        {
            new("mock-1001", "Alex Chen", "Friendly service and great haircut.", "Positive", "Thanks Alex! We appreciate the compliment!"),
            new("mock-1002", "Jordan Singh", "Very long wait time. Disappointed.", "Negative", "We're sorry about the delay, Jordan. We'll do better next time."),
            new("mock-1003", "Taylor Brooks", "Clean location, decent cut, but pricey.", "Neutral", "Thanks for the feedback, Taylor. We’ll review our pricing."),
            new("mock-1004", "Nina Patel", "Stylists were kind but rushed.", "Neutral", "Thanks Nina. We’ll work on pacing ourselves better."),
            new("mock-1005", "David Kim", "Fantastic fade and no wait!", "Positive", "Thanks David! Glad you enjoyed your visit."),
            new("mock-1006", "Sara Lopez", "Missed my appointment slot twice.", "Negative", "Sorry for the inconvenience Sara. We'll improve our scheduling."),
            new("mock-1007", "Chris Yang", "Best haircut I’ve had in years.", "Positive", "Appreciate it, Chris! Come back anytime."),
            new("mock-1008", "Jamie Tran", "Good cut, but music was too loud.", "Neutral", "Thanks Jamie. We’ll be mindful of volume levels."),
            new("mock-1009", "Ella Moore", "Receptionist was dismissive.", "Negative", "Sorry to hear that Ella. We'll coach our front desk team."),
            new("mock-1010", "Omar Reyes", "Excellent service, great vibes!", "Positive", "Thanks Omar! Always a pleasure having you.")
        };

        // GET: /api/mock/reviews
        [HttpGet("reviews")]
        public async Task<IActionResult> GetMockReviews(
            [FromQuery] bool simulateDelay = false,
            [FromQuery] bool simulateError = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (simulateError)
            {
                return StatusCode(500, new { error = "Simulated server error." });
            }

            if (simulateDelay)
            {
                await Task.Delay(1500);
            }

            var paged = MockReviews
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                data = paged,
                total = MockReviews.Count
            });
        }

        // POST: /api/mock/respond
        [HttpPost("respond")]
        public async Task<IActionResult> RespondToMockReview([FromBody] MockReviewResponseDto input)
        {
            if (string.IsNullOrWhiteSpace(input.ReviewId) || string.IsNullOrWhiteSpace(input.FinalResponse))
            {
                return BadRequest("ReviewId and FinalResponse are required.");
            }

            var review = MockReviews.FirstOrDefault(r => r.ReviewId == input.ReviewId);
            if (review is null)
            {
                return NotFound("Review not found.");
            }

            // Simulate DB update
            review.Status = "Responded";
            review.FinalResponse = input.FinalResponse;

            await Task.Delay(500); // Slight delay for realism
            Console.WriteLine($"✅ Responded to {review.ReviewId}: {input.FinalResponse}");

            return Ok(new { message = "Mock response submitted successfully." });
        }
        
        // POST: /api/mock/reset
        [HttpPost("reset")]
        public IActionResult ResetMockReviews()
        {
            foreach (var review in MockReviews)
            {
                review.Status = "Pending";
                review.FinalResponse = "";
            }

            return Ok(new { message = "Mock reviews reset to default state." });
        }

        // DTO
        public class MockReviewResponseDto
        {
            public string ReviewId { get; set; }
            public string FinalResponse { get; set; }
        }

        // Entity
        public class MockReview
        {
            public MockReview(string id, string author, string comment, string sentiment, string suggestedResponse)
            {
                ReviewId = id;
                Author = author;
                Comment = comment;
                Sentiment = sentiment;
                SuggestedResponse = suggestedResponse;
                Status = "Pending";
            }

            public string ReviewId { get; set; }
            public string Author { get; set; }
            public string Comment { get; set; }
            public string Sentiment { get; set; }
            public string SuggestedResponse { get; set; }
            public string FinalResponse { get; set; } = "";
            public string Status { get; set; } = "Pending";
        }
    }
}
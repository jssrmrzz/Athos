using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Athos.ReviewAutomation.Api.Controllers
{
    [ApiController]
    [Route("api/mock")]
    public class MockReviewApiController : ControllerBase
    {
        // 🧠 In-memory mock review list
        private static readonly List<MockReview> MockReviews = new()
        {
            new("rev1001", "Jane Abadayo", "FIVE", "Excellent experience! The staff was friendly and the haircut was perfect."),
            new("rev1002", "Mark Lin", "TWO", "Waited over 30 minutes past my appointment. Not impressed."),
            new("rev1003", "Samantha Lee", "FOUR", "Great cut and ambiance. A bit pricey though."),
            new("rev1004", "Carlos Alvarez", "THREE", "Okay experience. Stylist was nice but seemed rushed."),
            new("rev1005", "Tina Zhang", "FIVE", "I always leave feeling fresh and confident. Highly recommend!"),
            new("rev1006", "Devin Nguyen", "ONE", "Terrible service. They skipped my appointment and didn’t apologize."),
            new("rev1007", "Angela Park", "FOUR", "Friendly staff, relaxing environment. A bit of a wait though."),
            new("rev1008", "Michael B.", "THREE", "Haircut was fine, but they overcharged me."),
            new("rev1009", "Priya Kaur", "FIVE", "The stylist listened to exactly what I wanted. Love the result!"),
            new("rev1010", "Ravi Patel", "TWO", "Haircut felt rushed and uneven. Wouldn’t return."),
            new("rev1011", "Zoe Kim", "FIVE", "Clean, professional, and always on time. My go-to salon."),
            new("rev1012", "Daniel Wu", "THREE", "Stylist was polite but didn’t follow my instructions fully."),
            new("rev1013", "Nina Gomez", "FOUR", "Love the vibe! They even offered me coffee while I waited."),
            new("rev1014", "Liam Johnson", "ONE", "Music was too loud and staff seemed distracted."),
            new("rev1015", "Emily Nguyen", "FIVE", "Best barbershop in town. I always walk out smiling!")
        };

        // GET /api/mock/reviews
        [HttpGet("reviews")]
        public async Task<IActionResult> GetMockReviews(
            [FromQuery] bool simulateDelay = false,
            [FromQuery] bool simulateError = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (simulateError)
                return StatusCode(500, new { error = "Simulated server error." });

            if (simulateDelay)
                await Task.Delay(1500);

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

        // POST /api/mock/respond
        [HttpPost("respond")]
        public async Task<IActionResult> RespondToMockReview([FromBody] MockReviewResponseDto input)
        {
            if (string.IsNullOrWhiteSpace(input.ReviewId) || string.IsNullOrWhiteSpace(input.FinalResponse))
                return BadRequest("ReviewId and FinalResponse are required.");

            var review = MockReviews.FirstOrDefault(r => r.ReviewId == input.ReviewId);
            if (review is null)
                return NotFound("Review not found.");

            review.FinalResponse = input.FinalResponse;
            review.Status = "Responded";

            await Task.Delay(500);
            Console.WriteLine($"✅ Responded to {review.ReviewId}: {input.FinalResponse}");

            return Ok(new { message = "Mock response submitted successfully." });
        }

        // POST /api/mock/reset
        [HttpPost("reset")]
        public IActionResult ResetMockReviews()
        {
            foreach (var review in MockReviews)
            {
                review.FinalResponse = "";
                review.Status = "Pending";
            }

            return Ok(new { message = "Mock reviews reset to default state." });
        }

        // DTO used in POST /respond
        public class MockReviewResponseDto
        {
            public string ReviewId { get; set; }
            public string FinalResponse { get; set; }
        }

        // 🧩 Google-style Mock Review entity
        public class MockReview
        {
            public MockReview(string reviewId, string displayName, string starRating, string comment)
            {
                ReviewId = reviewId;
                Reviewer = new Reviewer { DisplayName = displayName };
                StarRating = starRating;
                Comment = comment;
                CreateTime = DateTime.UtcNow.AddDays(-5).ToString("o");
                UpdateTime = DateTime.UtcNow.ToString("o");
                Status = "Pending";
                FinalResponse = "";
                SuggestedResponse = "";
            }

            public string ReviewId { get; set; }
            public Reviewer Reviewer { get; set; }
            public string StarRating { get; set; } // "FIVE", "ONE", etc.
            public string Comment { get; set; }
            public string CreateTime { get; set; }
            public string UpdateTime { get; set; }
            public string Status { get; set; }
            public string FinalResponse { get; set; }
            public string SuggestedResponse { get; set; }
        }

        public class Reviewer
        {
            public string DisplayName { get; set; }
        }
    }
}
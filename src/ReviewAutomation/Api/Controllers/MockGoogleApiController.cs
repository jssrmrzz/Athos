using Microsoft.AspNetCore.Mvc;

namespace Athos.ReviewAutomation.Api.Controllers;

[ApiController]
[Route("mockgoogleapi")]
public class MockGoogleApiController : ControllerBase
{
    [HttpGet("mock-reviews")]
    public IActionResult GetReviews()
    {
        bool simulateFailure = false; //toggle to mock failure8

        if (simulateFailure)
        {
            return StatusCode(500, "Simulated API failure");
        }
        
        var reviews = new[]
{
    new {
        reviewId = "rev1001",
        reviewer = new { displayName = "Jane Doe" },
        starRating = "FIVE",
        comment = "Excellent experience!",
        createTime = DateTime.UtcNow.AddDays(-2).ToString("o"),
        updateTime = DateTime.UtcNow.AddDays(-1).ToString("o")
    },
    new {
        reviewId = "rev1002",
        reviewer = new { displayName = "Rob Murphy" },
        starRating = "TWO",
        comment = "Long wait times.",
        createTime = DateTime.UtcNow.AddDays(-5).ToString("o"),
        updateTime = DateTime.UtcNow.AddDays(-3).ToString("o")
    },
    new {
        reviewId = "rev1003",
        reviewer = new { displayName = "Anna Chen" },
        starRating = "FOUR",
        comment = "Friendly staff, clean space.",
        createTime = DateTime.UtcNow.AddDays(-6).ToString("o"),
        updateTime = DateTime.UtcNow.AddDays(-4).ToString("o")
    },
    new {
        reviewId = "rev1004",
        reviewer = new { displayName = "Marco Lopez" },
        starRating = "THREE",
        comment = "Average service. Could improve wait time.",
        createTime = DateTime.UtcNow.AddDays(-8).ToString("o"),
        updateTime = DateTime.UtcNow.AddDays(-7).ToString("o")
    },
    new {
        reviewId = "rev1005",
        reviewer = new { displayName = "Lisa Tran" },
        starRating = "ONE",
        comment = "Terrible customer service. Would not return.",
        createTime = DateTime.UtcNow.AddDays(-10).ToString("o"),
        updateTime = DateTime.UtcNow.AddDays(-9).ToString("o")
    },
    new {
        reviewId = "rev1006",
        reviewer = new { displayName = "Kevin Patel" },
        starRating = "FIVE",
        comment = "Absolutely loved it. Highly recommend!",
        createTime = DateTime.UtcNow.AddDays(-3).ToString("o"),
        updateTime = DateTime.UtcNow.AddDays(-2).ToString("o")
    },
    new {
        reviewId = "rev1007",
        reviewer = new { displayName = "Maria Gonzales" },
        starRating = "TWO",
        comment = "Not worth the price.",
        createTime = DateTime.UtcNow.AddDays(-15).ToString("o"),
        updateTime = DateTime.UtcNow.AddDays(-14).ToString("o")
    },
    new {
        reviewId = "rev1008",
        reviewer = new { displayName = "Sean Kim" },
        starRating = "THREE",
        comment = "Service was okay, nothing special.",
        createTime = DateTime.UtcNow.AddDays(-6).ToString("o"),
        updateTime = DateTime.UtcNow.AddDays(-5).ToString("o")
    },
    new {
        reviewId = "rev1009",
        reviewer = new { displayName = "Emily Zhang" },
        starRating = "FOUR",
        comment = "Quick service and helpful team.",
        createTime = DateTime.UtcNow.AddDays(-11).ToString("o"),
        updateTime = DateTime.UtcNow.AddDays(-10).ToString("o")
    },
    new {
        reviewId = "rev1010",
        reviewer = new { displayName = "George Thompson" },
        starRating = "ONE",
        comment = "Left without getting what I paid for.",
        createTime = DateTime.UtcNow.AddDays(-12).ToString("o"),
        updateTime = DateTime.UtcNow.AddDays(-11).ToString("o")
    }
};

        return Ok(reviews);
    }
}

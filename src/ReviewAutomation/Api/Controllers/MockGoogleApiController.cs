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
            }
        };

        return Ok(reviews);
    }
}

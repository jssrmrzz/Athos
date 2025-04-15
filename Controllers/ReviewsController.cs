using Athos.Api.Models;
using Athos.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Athos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewPollingService _pollingService;

        public ReviewsController()
        {
            _pollingService = new ReviewPollingService();
        }

        [HttpGet]
        public ActionResult<List<Review>> Get()
        {
            var reviews = _pollingService.GetReviews();
            return Ok(reviews);
        }

        [HttpPost("respond")]
        public ActionResult RespondToReview([FromBody] ReviewResponseDto input)
        {
            var reviews = _pollingService.GetReviews();
            var review = reviews.FirstOrDefault(r => r.ReviewId == input.ReviewId);

            if (review == null)
                return NotFound("Review not found.");

            review.FinalResponse = input.FinalResponse;
            review.IsApproved = true;

            Console.WriteLine($"✅ Response approved for review {review.ReviewId}:\n{input.FinalResponse}");

            return Ok(review);
        }
    }
}

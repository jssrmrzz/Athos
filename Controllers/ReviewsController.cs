using Athos.Api.Services;
using Athos.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Athos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewPollingService _pollingService;

        public ReviewsController(ReviewPollingService pollingService)
        {
            _pollingService = pollingService;
        }

        [HttpGet]
        public ActionResult<List<ReviewOutputDto>> Get()
        {
            var reviews = _pollingService.GetReviews();

            var reviewDtos = reviews.Select(r => new ReviewOutputDto
            {
                ReviewId = r.ReviewId ?? "",
                Author = r.Author ?? "",
                Rating = r.Rating,
                Comment = r.Comment ?? "",
                Sentiment = r.Sentiment,
                Status = r.Status,
                SuggestedResponse = r.SuggestedResponse,
                FinalResponse = r.FinalResponse,
                SubmittedAgo = r.SubmittedAgo,
                ApprovedAgo = r.ApprovedAgo
            }).ToList();

            return Ok(reviewDtos);
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

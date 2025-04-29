using Athos.Api.Data;
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
        private readonly ReviewApprovalService _approvalService;


        public ReviewsController(ReviewPollingService pollingService, ReviewApprovalService approvalService)
        {
            _pollingService = pollingService;
            _approvalService = approvalService;
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
            var (isSuccess, errorMessage) = _approvalService.ApproveReview(input.ReviewId, input.FinalResponse);

            if (!isSuccess)
            {
                return BadRequest(errorMessage);
            }

            return Ok();
        }
    }
}

using Athos.ReviewAutomation.Core.Services;
using Athos.ReviewAutomation.Infrastructure.Services;
using Athos.ReviewAutomation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Athos.ReviewAutomation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewPollingService _pollingService;
        private readonly ReviewApprovalService _approvalService;

        public ReviewsController(
            ReviewPollingService pollingService,
            ReviewApprovalService approvalService)
        {
            _pollingService = pollingService;
            _approvalService = approvalService;
        }

        // ✅ GET: Fetch reviews with optional filters and sorting
        [HttpGet]
        public ActionResult<List<ReviewOutputDto>> Get(
            [FromQuery] string? sentiment,
            [FromQuery] bool? isApproved,
            [FromQuery] string sortBy = "SubmittedAt",
            [FromQuery] string sortDirection = "desc")
        {
            // Validate sortBy input
            var validSortFields = new[] { "Rating", "SubmittedAt", "ApprovedAt" };
            if (!validSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
                return BadRequest($"❌ Invalid sortBy value. Use one of: {string.Join(", ", validSortFields)}");

            // Validate sortDirection input
            if (!string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase))
                return BadRequest("❌ Invalid sortDirection. Use 'asc' or 'desc'.");

            // Validate sentiment (if passed)
            if (!string.IsNullOrWhiteSpace(sentiment))
            {
                var validSentiments = new[] { "positive", "neutral", "negative" };
                if (!validSentiments.Contains(sentiment.ToLower()))
                    return BadRequest($"❌ Invalid sentiment. Allowed values: {string.Join(", ", validSentiments)}");
            }

            // Fetch and project results
            var reviews = _pollingService.GetReviews(sentiment, isApproved, sortBy, sortDirection);

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
                SubmittedAt = r.SubmittedAt,
                ApprovedAt = r.ApprovedAt,
                IsApproved = r.IsApproved 
            }).ToList();

            return Ok(reviewDtos);
        }

        // ✅ POST: Approve and respond to a review
        [HttpPost("respond")]
        public ActionResult RespondToReview([FromBody] ReviewResponseDto input)
        {
            var (isSuccess, errorMessage) = _approvalService.ApproveReview(input.ReviewId, input.FinalResponse);

            if (!isSuccess)
                return BadRequest(errorMessage);

            return Ok();
        }

        // ✅ POST: Trigger ingestion from Google mock API (version A)
        [HttpPost("import-google")]
        public async Task<ActionResult> ImportFromGoogle([FromServices] GoogleReviewIngestionService ingestionService)
        {
            var importedCount = await ingestionService.FetchAndSaveReviewsAsync();
            return Ok(new { message = $"{importedCount} new reviews imported." });
        }

        // ✅ POST: Alternate trigger for ingestion (version B - shorter response)
        [HttpPost("ingest")]
        public async Task<IActionResult> IngestReviews([FromServices] GoogleReviewIngestionService service)
        {
            var count = await service.FetchAndSaveReviewsAsync();
            return Ok($"✅ Ingested {count} reviews.");
        }
    }
}

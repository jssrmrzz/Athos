using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Application.DTOs.Reviews;
using Microsoft.AspNetCore.Mvc;
using Athos.ReviewAutomation.Application.UseCases.Reviews;
using Microsoft.AspNetCore.Authorization;

namespace Athos.ReviewAutomation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IGetReviewsUseCase _getReviews;
        private readonly IApproveReviewUseCase _approveReview;
        private readonly IBusinessContextService _businessContext;

        public ReviewsController(
            IGetReviewsUseCase getReviews,
            IApproveReviewUseCase approveReview,
            IBusinessContextService businessContext)
        {
            _getReviews = getReviews;
            _approveReview = approveReview;
            _businessContext = businessContext;
        }

        // ✅ GET: Fetch paginated, sorted, filtered reviews
        [HttpGet]
        public async Task<ActionResult<object>> Get(
            [FromQuery] string? sentiment,
            [FromQuery] bool? isApproved,
            [FromQuery] string sortBy = "SubmittedAt",
            [FromQuery] string sortDirection = "desc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var businessId = _businessContext.GetCurrentBusinessId();
            if (!businessId.HasValue)
                return BadRequest("❌ Business context not found");

            if (page < 1 || pageSize < 1)
                return BadRequest("❌ 'page' and 'pageSize' must be positive integers.");

            var validSortFields = new[] { "Rating", "SubmittedAt", "ApprovedAt" };
            if (!validSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
                return BadRequest($"❌ Invalid sortBy value. Use one of: {string.Join(", ", validSortFields)}");

            if (!string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase))
                return BadRequest("❌ Invalid sortDirection. Use 'asc' or 'desc'.");

            if (!string.IsNullOrWhiteSpace(sentiment))
            {
                var validSentiments = new[] { "positive", "neutral", "negative" };
                if (!validSentiments.Contains(sentiment.ToLower()))
                    return BadRequest($"❌ Invalid sentiment. Allowed values: {string.Join(", ", validSentiments)}");
            }

            // 🧠 Fetch and project
            var allReviews = await _getReviews.Execute(businessId.Value, sentiment, isApproved, sortBy, sortDirection);
            var totalCount = allReviews.Count;

            var pagedReviews = allReviews
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var reviewDtos = pagedReviews.Select(r => new GetReviewsResponseOutputDto
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

            return Ok(new
            {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                data = reviewDtos
            });
        }

        // ✅ POST: Finalize review response
        [HttpPost("respond")]
        public ActionResult RespondToReview([FromBody] ApproveReviewRequestDto input)
        {
            var (isSuccess, errorMessage) = _approveReview.Execute(input);
            return isSuccess ? Ok() : BadRequest(errorMessage);
        }

        // ✅ POST: Import from mock Google API
        [HttpPost("import-google")]
        public async Task<ActionResult> ImportFromGoogle([FromServices] IGoogleReviewIngestionService ingestionService)
        {
            var importedCount = await ingestionService.FetchAndSaveReviewsAsync();
            return Ok(new { message = $"{importedCount} new reviews imported." });
        }

        // ✅ POST: Alternate endpoint for import
        [HttpPost("ingest")]
        public async Task<IActionResult> IngestReviews([FromServices] IGoogleReviewIngestionService service)
        {
            var count = await service.FetchAndSaveReviewsAsync();
            return Ok($"✅ Ingested {count} reviews.");
        }
    }
}
using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Athos.ReviewAutomation.Application.UseCases.Reviews;



namespace Athos.ReviewAutomation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IGetReviewsUseCase _getReviews;
        private readonly IApproveReviewUseCase _approveReview;

        public ReviewsController(
            IGetReviewsUseCase getReviews,
            IApproveReviewUseCase approveReview)
        {
            _getReviews = getReviews;
            _approveReview = approveReview;
        }


        // ✅ GET: Fetch reviews with optional filters, sorting, and pagination
        [HttpGet]
        public ActionResult<object> Get(
            [FromQuery] string? sentiment,
            [FromQuery] bool? isApproved,
            [FromQuery] string sortBy = "SubmittedAt",
            [FromQuery] string sortDirection = "desc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10) 
        {
        // Validate pagination
        if (page < 1 || pageSize < 1)
            return BadRequest("❌ 'page' and 'pageSize' must be positive integers.");

        // Validate sortBy
        var validSortFields = new[] { "Rating", "SubmittedAt", "ApprovedAt" };
        if (!validSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            return BadRequest($"❌ Invalid sortBy value. Use one of: {string.Join(", ", validSortFields)}");

        // Validate sortDirection
        if (!string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase))
            return BadRequest("❌ Invalid sortDirection. Use 'asc' or 'desc'.");

        // Validate sentiment
        if (!string.IsNullOrWhiteSpace(sentiment))
        {
            var validSentiments = new[] { "positive", "neutral", "negative" };
            if (!validSentiments.Contains(sentiment.ToLower()))
                return BadRequest($"❌ Invalid sentiment. Allowed values: {string.Join(", ", validSentiments)}");
        }
    
        // Fetch filtered and sorted reviews
        var allReviews = _getReviews.Execute(sentiment, isApproved, sortBy, sortDirection);
        var totalCount = allReviews.Count;

        // Apply pagination
        var pagedReviews = allReviews
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var reviewDtos = pagedReviews.Select(r => new ReviewOutputDto
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

        // Return structured response
        return Ok(new
        {
            page,
            pageSize,
            totalCount,
            totalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            data = reviewDtos
        });
    }


        // ✅ POST: Approve and finalize a review response
        [HttpPost("respond")]
        public ActionResult RespondToReview([FromBody] ReviewResponseDto input)
        {
            var (isSuccess, errorMessage) = _approveReview.Execute(input);
            return isSuccess ? Ok() : BadRequest(errorMessage);
        }

        // ✅ POST: Import reviews from Google Mock API
        [HttpPost("import-google")]
        public async Task<ActionResult> ImportFromGoogle([FromServices] IGoogleReviewIngestionService ingestionService)
        {
            var importedCount = await ingestionService.FetchAndSaveReviewsAsync();
            return Ok(new { message = $"{importedCount} new reviews imported." });
        }

        // ✅ POST: Alternative import endpoint (simpler response)
        [HttpPost("ingest")]
        public async Task<IActionResult> IngestReviews([FromServices] IGoogleReviewIngestionService service)
        {
            var count = await service.FetchAndSaveReviewsAsync();
            return Ok($"✅ Ingested {count} reviews.");
        }
    }
}

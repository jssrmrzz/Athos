using Athos.ReviewAutomation.Core.Entities;
using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Core.Services;
using Athos.ReviewAutomation.Infrastructure.Services;

public class ReviewPollingService : IReviewPollingService
{
    private readonly IReviewRepository _repo;
    private readonly SentimentService _sentimentService = new();
    private readonly AutoReplyService _autoReplyService = new();
    private readonly NotificationService _notificationService = new();
    private readonly GoogleReviewClient _googleReviewClient;
    private readonly ILlmClient _llmClient;

    public ReviewPollingService(
        IReviewRepository repo,
        GoogleReviewClient googleReviewClient,
        ILlmClient llmClient)
    {
        _repo = repo;
        _googleReviewClient = googleReviewClient;
        _llmClient = llmClient;

        // Seed mock reviews on first run if DB is empty
        _repo.SeedReviewsFromJsonIfEmpty();
    }

    public async Task<List<DbReview>> GetReviews(string? sentiment, bool? isApproved, string? sortBy, string? sortDirection)
    {
        var reviews = _repo.GetAllReviews().AsQueryable();

        // Optional filters
        if (!string.IsNullOrWhiteSpace(sentiment))
            reviews = reviews.Where(r => r.Sentiment.Equals(sentiment, StringComparison.OrdinalIgnoreCase));

        if (isApproved.HasValue)
            reviews = reviews.Where(r => r.IsApproved == isApproved.Value);

        // Normalize sort input
        sortBy = sortBy?.ToLower();
        sortDirection = sortDirection?.ToLower();
        bool desc = sortDirection == "desc";

        // Sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            reviews = sortBy switch
            {
                "rating" => desc ? reviews.OrderByDescending(r => r.Rating) : reviews.OrderBy(r => r.Rating),
                "submittedat" => desc ? reviews.OrderByDescending(r => r.SubmittedAt) : reviews.OrderBy(r => r.SubmittedAt),
                "approvedat" => desc ? reviews.OrderByDescending(r => r.ApprovedAt) : reviews.OrderBy(r => r.ApprovedAt),
                _ => reviews
            };
        }

        // Enrich each review
        foreach (var review in reviews)
        {
            review.Sentiment = _sentimentService.AnalyzeSentiment(review.Rating, review.Comment);

            // 🔁 Replace hardcoded logic with LLM-powered response
            // Only auto-generate a polite reply for non-negative reviews
            if (string.IsNullOrWhiteSpace(review.SuggestedResponse))
            {
                // Only generate if it's missing
                review.SuggestedResponse = await _llmClient.GenerateResponseAsync(review.Comment);
            }

            if (review.Sentiment == "Negative" && !review.AlertSent)
            {
                _notificationService.SendAlert(review);
                review.AlertSent = true;
            }
        }

        _repo.SaveChanges();
        return reviews.ToList();
    }

    public async Task FetchAndStoreGoogleReviewsAsync()
    {
        var externalReviews = await _googleReviewClient.FetchReviewsAsync();

        var dbReviews = externalReviews.Select(r => new DbReview
        {
            ReviewId = r.ReviewId,
            Author = r.Reviewer?.DisplayName ?? "Anonymous",
            Rating = StarRatingToInt(r.StarRating),
            Comment = r.Comment,
            SubmittedAt = DateTime.Parse(r.CreateTime),
            FinalResponse = r.ReviewReply?.Comment,
            ApprovedAt = string.IsNullOrEmpty(r.ReviewReply?.UpdateTime)
                ? null
                : DateTime.Parse(r.ReviewReply.UpdateTime),
            IsApproved = r.ReviewReply != null
        }).ToList();

        _repo.AddReviewsIfNotExists(dbReviews);
    }

    // Converts Google enum star rating strings to integer
    private int StarRatingToInt(string? rating)
    {
        return rating?.ToLowerInvariant() switch
        {
            "one" => 1,
            "two" => 2,
            "three" => 3,
            "four" => 4,
            "five" => 5,
            _ => 0
        };
    }
}
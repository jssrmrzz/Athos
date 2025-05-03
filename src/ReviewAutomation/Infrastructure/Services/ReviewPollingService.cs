using Athos.ReviewAutomation.Core.Services;
using Athos.ReviewAutomation.Infrastructure.Repositories;
using Athos.ReviewAutomation.Models;

namespace Athos.ReviewAutomation.Infrastructure.Services
{
    public class ReviewPollingService
    {
        private readonly ReviewRepository _repo;
        private readonly SentimentService _sentimentService = new();
        private readonly AutoReplyService _autoReplyService = new();
        private readonly NotificationService _notificationService = new();
        private readonly GoogleReviewClient _googleReviewClient;
        
        public ReviewPollingService(ReviewRepository repo, GoogleReviewClient googleReviewClient)
        {
            _repo = repo;

            // Seed mock reviews if needed
            _repo.SeedReviewsFromJsonIfEmpty();

            _googleReviewClient = googleReviewClient;
        }

        public List<DbReview> GetReviews(string? sentiment, bool? isApproved, string? sortBy, string? sortDirection)
        {
            var reviews = _repo.GetAllReviews().AsQueryable();

            // Filter by sentiment
            if (!string.IsNullOrWhiteSpace(sentiment))
                reviews = reviews.Where(r => r.Sentiment.Equals(sentiment, StringComparison.OrdinalIgnoreCase));

            // Filter by approval status
            if (isApproved.HasValue)
                reviews = reviews.Where(r => r.IsApproved == isApproved.Value);

            // Normalize inputs
            sortBy = sortBy?.ToLower();
            sortDirection = sortDirection?.ToLower();
            bool desc = sortDirection == "desc";

            // Sort
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                reviews = sortBy switch
                {
                    "Rating" => desc ? reviews.OrderByDescending(r => r.Rating) : reviews.OrderBy(r => r.Rating),
                    "SubmittedAt" => desc ? reviews.OrderByDescending(r => r.SubmittedAt) : reviews.OrderBy(r => r.SubmittedAt),
                    "ApprovedAt" => desc ? reviews.OrderByDescending(r => r.ApprovedAt) : reviews.OrderBy(r => r.ApprovedAt),
                    _ => reviews // fallback to no sorting
                };
            }

            // Enrich and alert
            foreach (var review in reviews)
            {
                review.Sentiment = _sentimentService.AnalyzeSentiment(review.Rating, review.Comment);
                review.SuggestedResponse = _autoReplyService.GenerateReply(review.Sentiment, review.Author);

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
}

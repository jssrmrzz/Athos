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

        public ReviewPollingService(ReviewRepository repo)
        {
            _repo = repo;

            // Seed mock reviews if needed
            _repo.SeedReviewsFromJsonIfEmpty();
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
    }
}

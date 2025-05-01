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

            // Move seeding responsibility to repository
            _repo.SeedReviewsFromJsonIfEmpty();
        }

        public List<DbReview> GetReviews()
        {
            var reviews = _repo.GetAllReviews();

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

            _repo.SaveChanges(); // Persist any updates
            return reviews;
        }
    }
}

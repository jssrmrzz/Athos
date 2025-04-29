using Athos.Api.Data;
using Athos.Api.Models;
using System.Text.Json;

namespace Athos.Api.Services
{
    public class ReviewPollingService
    {
        private readonly SentimentService _sentimentService = new SentimentService();
        private readonly AutoReplyService _autoReplyService = new AutoReplyService();
        private readonly NotificationService _notificationService = new NotificationService();
        private readonly ReviewDbContext _db;

        private readonly string _mockDataPath = "Data/mockGoogleReviews.json";

        public ReviewPollingService(ReviewDbContext dbContext)
        {
            _db = dbContext;

            // Optionally seed DB if empty
            if (!_db.Reviews.Any())
            {
                SeedReviewsFromJson();
            }
        }

        public List<DbReview> GetReviews()
        {
            var reviews = _db.Reviews.ToList();

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

            _db.SaveChanges(); // Persist any updates

            return reviews;
        }

        private void SeedReviewsFromJson()
        {
            if (!File.Exists(_mockDataPath)) return;

            var json = File.ReadAllText(_mockDataPath);
            var reviews = JsonSerializer.Deserialize<List<DbReview>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (reviews == null) return;

            _db.Reviews.AddRange(reviews);
            _db.SaveChanges();
        }
    }
}

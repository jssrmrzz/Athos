using Athos.Api.Models;
using System.Text.Json;

namespace Athos.Api.Services
{
    public class ReviewPollingService
    {
        private readonly string _mockDataPath = "Data/mockGoogleReviews.json";
        private readonly SentimentService _sentimentService = new SentimentService();
        private readonly AutoReplyService _autoReplyService = new AutoReplyService();
        private readonly NotificationService _notificationService = new NotificationService();


        public List<Review> GetReviews()
        {
            if (!File.Exists(_mockDataPath))
                return new List<Review>();

            var json = File.ReadAllText(_mockDataPath);
            var reviews = JsonSerializer.Deserialize<List<Review>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (reviews == null) return new List<Review>();

            foreach (var review in reviews)
            {
                review.Sentiment = _sentimentService.AnalyzeSentiment(review.Rating, review.Comment);
                review.SuggestedResponse = _autoReplyService.GenerateReply(review.Sentiment, review.Author);

                if (review.Sentiment == "Negative" && !review.AlertSent)
                {
                    _notificationService.SendAlert(review);
                    review.AlertSent = true; //Mark as alerted
                }
            }

            return reviews;
        }
    }
}

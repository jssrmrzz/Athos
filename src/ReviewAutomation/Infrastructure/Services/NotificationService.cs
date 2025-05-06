using Athos.ReviewAutomation.Core.Entities;


namespace Athos.ReviewAutomation.Infrastructure.Services
{
    public class NotificationService
    {
        public void SendAlert(DbReview review)
        {
            Console.WriteLine("🚨 ALERT: Negative review received");
            Console.WriteLine($"- Author: {review.Author}");
            Console.WriteLine($"- Comment: {review.Comment}");
            Console.WriteLine($"- Rating: {review.Rating}");
            Console.WriteLine($"- Timestamp: {review.Timestamp}");
        }
    }
}

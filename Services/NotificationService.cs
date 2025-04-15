using Athos.Api.Models;

namespace Athos.Api.Services
{
    public class NotificationService
    {
        public void SendAlert(Review review)
        {
            Console.WriteLine("🚨 ALERT: Negative review received");
            Console.WriteLine($"- Author: {review.Author}");
            Console.WriteLine($"- Comment: {review.Comment}");
            Console.WriteLine($"- Rating: {review.Rating}");
            Console.WriteLine($"- Timestamp: {review.Timestamp}");
        }
    }
}

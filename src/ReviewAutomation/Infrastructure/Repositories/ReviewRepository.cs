using System.Text.Json;
using Athos.ReviewAutomation.Infrastructure.Data;
using Athos.ReviewAutomation.Models;



namespace Athos.ReviewAutomation.Infrastructure.Repositories
{
    public class ReviewRepository
    {
        private readonly ReviewDbContext _db;

        public ReviewRepository(ReviewDbContext db)
        {
            _db = db;
        }

        public List<DbReview> GetAllReviews()
        {
            return _db.Reviews.ToList();
        }
        
        public void SeedReviewsFromJsonIfEmpty()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "mockGoogleReviews.json");


            if (_db.Reviews.Any()) return;

            if (!File.Exists(path))
            {
                Console.WriteLine($"❌ Seed file not found at {path}");
                return;
            }

            var json = File.ReadAllText(path);
            var reviews = JsonSerializer.Deserialize<List<DbReview>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (reviews == null || reviews.Count == 0)
            {
                Console.WriteLine("⚠️ No reviews to seed.");
                return;
            }

            _db.Reviews.AddRange(reviews);
            _db.SaveChanges();
            Console.WriteLine($"✅ Seeded {reviews.Count} reviews.");
        }


        public void SaveChanges()
        {
            _db.SaveChanges();
        }
        
        public void AddReviews(List<DbReview> reviews)
        {
            _db.Reviews.AddRange(reviews);
        }
        
        public void AddReviewsIfNotExists(List<DbReview> reviews)
        {
            foreach (var review in reviews)
            {
                if (!_db.Reviews.Any(r => r.ReviewId == review.ReviewId))
                {
                    _db.Reviews.Add(review);
                }
            }

            _db.SaveChanges();
        }



    }
}
using System.Text.Json;
using Athos.ReviewAutomation.Core.Entities;
using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Infrastructure.Data;



namespace Athos.ReviewAutomation.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ReviewDbContext _db;

        public ReviewRepository(ReviewDbContext db)
        {
            _db = db;
        }

        public List<DbReview> GetAllReviews(int businessId)
        {
            return _db.Reviews.Where(r => r.BusinessId == businessId).ToList();
        }
        
        public List<DbReview> GetAllReviews()
        {
            return _db.Reviews.ToList();
        }
        
        public void SeedReviewsFromJsonIfEmpty(int? businessId = null)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "mockGoogleReviews.json");

            // If businessId is provided, check if that business has reviews
            if (businessId.HasValue && _db.Reviews.Any(r => r.BusinessId == businessId.Value)) return;
            
            // If no businessId provided, check if any reviews exist
            if (!businessId.HasValue && _db.Reviews.Any()) return;

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

            // If businessId is provided, assign all reviews to that business
            if (businessId.HasValue)
            {
                foreach (var review in reviews)
                {
                    review.BusinessId = businessId.Value;
                }
            }
            else
            {
                // For migration: assign to default business (ID 1) or create one
                foreach (var review in reviews)
                {
                    review.BusinessId = 1; // Default business for migration
                }
            }

            _db.Reviews.AddRange(reviews);
            _db.SaveChanges();
            Console.WriteLine($"✅ Seeded {reviews.Count} reviews for business {businessId ?? 1}.");
        }


        public void SaveChanges()
        {
            _db.SaveChanges();
        }
        
        public void AddReviews(List<DbReview> reviews)
        {
            _db.Reviews.AddRange(reviews);
        }
        
        public void AddReviewsIfNotExists(List<DbReview> reviews, int businessId)
        {
            foreach (var review in reviews)
            {
                if (!_db.Reviews.Any(r => r.ReviewId == review.ReviewId && r.BusinessId == businessId))
                {
                    review.BusinessId = businessId;
                    _db.Reviews.Add(review);
                }
            }

            _db.SaveChanges();
        }

        public DbReview? GetReview(string reviewId, int businessId)
        {
            return _db.Reviews.FirstOrDefault(r => r.ReviewId == reviewId && r.BusinessId == businessId);
        }

        public void UpdateReview(DbReview review)
        {
            _db.Reviews.Update(review);
        }



    }
}
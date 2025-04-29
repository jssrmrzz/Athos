using System.Text.Json;
using Athos.Api.Models;
using Athos.Api.Data;

namespace Athos.Api.Services
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
        
        public void SeedReviewsFromJsonIfEmpty(string path = "Data/mockGoogleReviews.json")
        {
            if (_db.Reviews.Any()) return;

            if (!File.Exists(path)) return;

            var json = File.ReadAllText(path);
            var reviews = JsonSerializer.Deserialize<List<DbReview>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (reviews == null) return;

            _db.Reviews.AddRange(reviews);
            _db.SaveChanges();
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }

    }
}
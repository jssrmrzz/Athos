using Athos.ReviewAutomation.Core.Entities;

namespace Athos.ReviewAutomation.Core.Interfaces
{
    public interface IReviewRepository
    {
        // Returns all reviews for a specific business
        List<DbReview> GetAllReviews(int businessId);
        
        // Returns all reviews (for migration purposes only)
        List<DbReview> GetAllReviews();
        
        // Adds a list of reviews to the database without checking for duplicates
        void AddReviews(List<DbReview> reviews);

        // Adds reviews that don't already exist based on ReviewId for a specific business
        void AddReviewsIfNotExists(List<DbReview> newReviews, int businessId);

        // Seeds initial data from local JSON if the DB is empty
        void SeedReviewsFromJsonIfEmpty(int? businessId = null);

        // Gets a specific review by ID for a business
        DbReview? GetReview(string reviewId, int businessId);

        // Updates a review
        void UpdateReview(DbReview review);

        // Persists changes to the database
        void SaveChanges();
    }
}
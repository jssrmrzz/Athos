using Athos.ReviewAutomation.Core.Entities;

namespace Athos.ReviewAutomation.Core.Interfaces
{
    public interface IReviewRepository
    {
        // Returns all reviews currently in the database
        List<DbReview> GetAllReviews();
        
        // Adds a list of reviews to the database without checking for duplicates
        void AddReviews(List<DbReview> reviews);

        // Adds reviews that don't already exist based on ReviewId
        void AddReviewsIfNotExists(List<DbReview> newReviews);

        // Seeds initial data from local JSON if the DB is empty
        void SeedReviewsFromJsonIfEmpty();

        // Persists changes to the database
        void SaveChanges();
    }
}
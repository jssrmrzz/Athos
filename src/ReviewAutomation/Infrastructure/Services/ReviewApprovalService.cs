using Athos.ReviewAutomation.Infrastructure.Data;

namespace Athos.ReviewAutomation.Infrastructure.Services
{
    public class ReviewApprovalService
    {
        private readonly ReviewDbContext _db;

        public ReviewApprovalService(ReviewDbContext db)
        {
            _db = db;
        }

        public (bool isSuccess, string errorMessage) ApproveReview(string reviewId, string finalResponse)
        {
            var review = _db.Reviews.FirstOrDefault(r => r.ReviewId == reviewId);

            if (review == null)
            {
                return (false, "Review not found.");
            }

            if (review.IsApproved)
            {
                return (false, "Review has already been approved.");
            }

            review.FinalResponse = finalResponse;
            review.IsApproved = true;
            review.ApprovedAt = DateTime.UtcNow;

            _db.SaveChanges();

            return (true, "");
        }
    }
}
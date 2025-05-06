using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Infrastructure.Data;

namespace Athos.ReviewAutomation.Application.UseCases
{
    public class ReviewApprovalService : IReviewApprovalService
    {
        private readonly ReviewDbContext _db;

        public ReviewApprovalService(ReviewDbContext db)
        {
            _db = db;
        }

        public (bool isSuccess, string? errorMessage) ApproveReview(string reviewId, string finalResponse)
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

            if (string.IsNullOrWhiteSpace(finalResponse))
            {
                return (false, "Final response cannot be empty.");
            }

            review.FinalResponse = finalResponse;
            review.IsApproved = true;
            review.ApprovedAt = DateTime.UtcNow;

            _db.SaveChanges();

            return (true, null);
        }

    }
}
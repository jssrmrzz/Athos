using Athos.ReviewAutomation.Models;

namespace Athos.ReviewAutomation.Core.Entities
{
    public interface IReviewPollingService
    {
        List<DbReview> GetReviews(string? sentiment, bool? isApproved, string? sortBy, string? sortDirection);
    }
}
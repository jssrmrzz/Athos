using Athos.ReviewAutomation.Models;

namespace Athos.ReviewAutomation.Core.Entities
{
    public interface IReviewPollingService
    {
        Task<List<DbReview>> GetReviews(int businessId, string? sentiment, bool? isApproved, string? sortBy, string? sortDirection);
    }
}
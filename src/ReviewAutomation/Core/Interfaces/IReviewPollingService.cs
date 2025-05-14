using Athos.ReviewAutomation.Models;

namespace Athos.ReviewAutomation.Core.Entities
{
    public interface IReviewPollingService
    {
        Task<List<DbReview>> GetReviews(string? sentiment, bool? isApproved, string? sortBy, string? sortDirection);
    }
}
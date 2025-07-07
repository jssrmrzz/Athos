using Athos.ReviewAutomation.Core.Entities;

namespace Athos.ReviewAutomation.Application.UseCases.Reviews
{
    public interface IGetReviewsUseCase
    {
        Task<List<DbReview>> Execute(int businessId, string? sentiment, bool? isApproved, string? sortBy, string? sortDirection);
    }
    
}
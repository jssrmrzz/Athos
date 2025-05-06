using Athos.ReviewAutomation.Core.Entities;

namespace Athos.ReviewAutomation.Application.UseCases.Reviews
{
    public interface IGetReviewsUseCase
    {
        List<DbReview> Execute(string? sentiment, bool? isApproved, string? sortBy, string? sortDirection);
    }
    
}
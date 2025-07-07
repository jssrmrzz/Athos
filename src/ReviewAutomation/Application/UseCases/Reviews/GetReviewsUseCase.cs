using Athos.ReviewAutomation.Core.Entities;

namespace Athos.ReviewAutomation.Application.UseCases.Reviews
{
    public class GetReviewsUseCase : IGetReviewsUseCase
    {
        private readonly IReviewPollingService _pollingService;

        public GetReviewsUseCase(IReviewPollingService pollingService)
        {
            _pollingService = pollingService;
        }

        public async Task<List<DbReview>> Execute(int businessId, string? sentiment, bool? isApproved, string? sortBy, string? sortDirection)
        {
            return await _pollingService.GetReviews(businessId, sentiment, isApproved, sortBy, sortDirection);
        }
    }
}
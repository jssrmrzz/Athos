using Athos.ReviewAutomation.Core.DTOs;
using Athos.ReviewAutomation.Infrastructure.Services;
using Athos.ReviewAutomation.Models;

namespace Athos.ReviewAutomation.Application.UseCases.Reviews
{
    public class ApproveReviewUseCase : IApproveReviewUseCase
    {
        private readonly ReviewApprovalService _approvalService;

        public ApproveReviewUseCase(ReviewApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        public (bool isSuccess, string errorMessage) Execute(ReviewResponseDto input)
        {
            return _approvalService.ApproveReview(input.ReviewId, input.FinalResponse);
        }
    }
}
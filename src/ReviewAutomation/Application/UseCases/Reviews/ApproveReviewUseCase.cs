using Athos.ReviewAutomation.Application.DTOs;
using Athos.ReviewAutomation.Application.DTOs.Reviews;
using Athos.ReviewAutomation.Core.Interfaces;

namespace Athos.ReviewAutomation.Application.UseCases.Reviews
{
    public class ApproveReviewUseCase : IApproveReviewUseCase
    {
        private readonly IReviewApprovalService _approvalService;

        public ApproveReviewUseCase(IReviewApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        public (bool isSuccess, string errorMessage) Execute(ApproveReviewRequestDto input)
        {
            return _approvalService.ApproveReview(input.ReviewId, input.FinalResponse);
        }
    }
}
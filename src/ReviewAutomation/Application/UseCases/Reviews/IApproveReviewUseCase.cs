using Athos.ReviewAutomation.Application.DTOs.Reviews;

namespace Athos.ReviewAutomation.Application.UseCases.Reviews
{
    public interface IApproveReviewUseCase
    {
        (bool isSuccess, string errorMessage) Execute(ReviewResponseDto input);
    }
}
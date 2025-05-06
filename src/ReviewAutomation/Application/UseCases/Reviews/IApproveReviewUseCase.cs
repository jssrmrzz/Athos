using Athos.ReviewAutomation.Models;

namespace Athos.ReviewAutomation.Application.UseCases.Reviews
{
    public interface IApproveReviewUseCase
    {
        (bool isSuccess, string errorMessage) Execute(ReviewResponseDto input);
    }
}
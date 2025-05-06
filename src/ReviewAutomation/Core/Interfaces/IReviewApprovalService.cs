namespace Athos.ReviewAutomation.Core.Interfaces
{
    public interface IReviewApprovalService
    {
        (bool isSuccess, string errorMessage) ApproveReview(string reviewId, string finalResponse);
    }
}
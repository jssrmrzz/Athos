namespace Athos.ReviewAutomation.Core.Interfaces
{
    public interface IGoogleReviewIngestionService
    {
        Task<int> FetchAndSaveReviewsAsync();
    }
}
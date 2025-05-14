namespace Athos.ReviewAutomation.Core.Interfaces
{
    public interface ILlmClient
    {
        Task<string> GenerateResponseAsync(string reviewText);
    }
}
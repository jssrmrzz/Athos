using Athos.ReviewAutomation.Core.Entities;

namespace Athos.ReviewAutomation.Core.Interfaces
{
    public interface IOAuthTokenRepository
    {
        Task<BusinessOAuthToken?> GetByBusinessIdAsync(int businessId, string provider = "Google");
        Task<BusinessOAuthToken> SaveAsync(BusinessOAuthToken token);
        Task<bool> DeleteByBusinessIdAsync(int businessId, string provider = "Google");
        Task<bool> RevokeByBusinessIdAsync(int businessId, string provider = "Google");
        Task<List<BusinessOAuthToken>> GetExpiredTokensAsync();
        Task<List<BusinessOAuthToken>> GetActiveTokensByProviderAsync(string provider = "Google");
    }
}
using Athos.ReviewAutomation.Core.Entities;

namespace Athos.ReviewAutomation.Core.Interfaces
{
    public interface IGoogleOAuthService
    {
        string GetAuthorizationUrl(int businessId, string state = "");
        Task<BusinessOAuthToken> ExchangeCodeForTokenAsync(int businessId, string code);
        Task<BusinessOAuthToken> RefreshTokenAsync(int businessId);
        Task<bool> RevokeTokenAsync(int businessId);
        Task<bool> IsTokenValidAsync(int businessId);
        Task<BusinessOAuthToken?> GetTokenAsync(int businessId);
        Task<string?> GetValidAccessTokenAsync(int businessId);
        Task<GoogleUserProfile?> GetUserProfileAsync(int businessId);
    }
}
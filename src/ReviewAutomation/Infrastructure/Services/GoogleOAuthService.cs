using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Athos.ReviewAutomation.Core.Entities;
using Athos.ReviewAutomation.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Athos.ReviewAutomation.Infrastructure.Services
{
    public class GoogleOAuthService : IGoogleOAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOAuthTokenRepository _oauthTokenRepository;
        private readonly ILogger<GoogleOAuthService> _logger;

        private const string GoogleAuthBaseUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string GoogleTokenUrl = "https://oauth2.googleapis.com/token";
        private const string GoogleRevokeUrl = "https://oauth2.googleapis.com/revoke";

        public GoogleOAuthService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IOAuthTokenRepository oauthTokenRepository,
            ILogger<GoogleOAuthService> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _oauthTokenRepository = oauthTokenRepository;
            _logger = logger;
        }

        public string GetAuthorizationUrl(int businessId, string state = "")
        {
            var clientId = _configuration["GoogleOAuth:ClientId"];
            var redirectUri = _configuration["GoogleOAuth:RedirectUri"];
            var scopes = _configuration.GetSection("GoogleOAuth:Scopes").Get<string[]>();

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri) || scopes == null)
            {
                throw new InvalidOperationException("Google OAuth configuration is missing");
            }

            var scopeString = string.Join(" ", scopes);
            var stateParam = string.IsNullOrEmpty(state) ? businessId.ToString() : $"{businessId}:{state}";

            var parameters = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["redirect_uri"] = redirectUri,
                ["response_type"] = "code",
                ["scope"] = scopeString,
                ["state"] = stateParam,
                ["access_type"] = "offline",
                ["prompt"] = "consent"
            };

            var queryString = string.Join("&", parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
            return $"{GoogleAuthBaseUrl}?{queryString}";
        }

        public async Task<BusinessOAuthToken> ExchangeCodeForTokenAsync(int businessId, string code)
        {
            var clientId = _configuration["GoogleOAuth:ClientId"];
            var clientSecret = _configuration["GoogleOAuth:ClientSecret"];
            var redirectUri = _configuration["GoogleOAuth:RedirectUri"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(redirectUri))
            {
                throw new InvalidOperationException("Google OAuth configuration is missing");
            }

            var parameters = new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code"
            };

            var tokenResponse = await MakeTokenRequestAsync(parameters);
            var token = CreateBusinessOAuthToken(businessId, tokenResponse);

            return await _oauthTokenRepository.SaveAsync(token);
        }

        public async Task<BusinessOAuthToken> RefreshTokenAsync(int businessId)
        {
            var existingToken = await _oauthTokenRepository.GetByBusinessIdAsync(businessId);
            if (existingToken?.RefreshToken == null)
            {
                throw new InvalidOperationException("No refresh token available for this business");
            }

            var clientId = _configuration["GoogleOAuth:ClientId"];
            var clientSecret = _configuration["GoogleOAuth:ClientSecret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new InvalidOperationException("Google OAuth configuration is missing");
            }

            var parameters = new Dictionary<string, string>
            {
                ["refresh_token"] = existingToken.RefreshToken,
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["grant_type"] = "refresh_token"
            };

            var tokenResponse = await MakeTokenRequestAsync(parameters);
            var token = CreateBusinessOAuthToken(businessId, tokenResponse);
            
            // Preserve the refresh token if not returned in response
            if (string.IsNullOrEmpty(token.RefreshToken))
            {
                token.RefreshToken = existingToken.RefreshToken;
            }

            return await _oauthTokenRepository.SaveAsync(token);
        }

        public async Task<bool> RevokeTokenAsync(int businessId)
        {
            var token = await _oauthTokenRepository.GetByBusinessIdAsync(businessId);
            if (token == null)
            {
                return false;
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var revokeUrl = $"{GoogleRevokeUrl}?token={Uri.EscapeDataString(token.AccessToken)}";
                var response = await httpClient.PostAsync(revokeUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    await _oauthTokenRepository.RevokeByBusinessIdAsync(businessId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to revoke token for business {BusinessId}: {StatusCode}", businessId, response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking token for business {BusinessId}", businessId);
                return false;
            }
        }

        public async Task<bool> IsTokenValidAsync(int businessId)
        {
            var token = await _oauthTokenRepository.GetByBusinessIdAsync(businessId);
            return token?.IsValid == true;
        }

        public async Task<BusinessOAuthToken?> GetTokenAsync(int businessId)
        {
            return await _oauthTokenRepository.GetByBusinessIdAsync(businessId);
        }

        public async Task<string?> GetValidAccessTokenAsync(int businessId)
        {
            var token = await _oauthTokenRepository.GetByBusinessIdAsync(businessId);
            if (token == null || token.IsRevoked)
            {
                return null;
            }

            if (token.IsExpired)
            {
                try
                {
                    token = await RefreshTokenAsync(businessId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to refresh token for business {BusinessId}", businessId);
                    return null;
                }
            }

            return token.AccessToken;
        }

        private async Task<TokenResponse> MakeTokenRequestAsync(Dictionary<string, string> parameters)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var formContent = new FormUrlEncodedContent(parameters);

            var response = await httpClient.PostAsync(GoogleTokenUrl, formContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Token request failed: {StatusCode} - {Content}", response.StatusCode, responseContent);
                throw new InvalidOperationException($"Token request failed: {response.StatusCode}");
            }

            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return tokenResponse ?? throw new InvalidOperationException("Failed to parse token response");
        }

        private BusinessOAuthToken CreateBusinessOAuthToken(int businessId, TokenResponse tokenResponse)
        {
            var scopes = _configuration.GetSection("GoogleOAuth:Scopes").Get<string[]>();
            var scopeString = scopes != null ? string.Join(" ", scopes) : "";

            return new BusinessOAuthToken
            {
                BusinessId = businessId,
                Provider = "Google",
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                Scope = scopeString
            };
        }

        private class TokenResponse
        {
            public string AccessToken { get; set; } = string.Empty;
            public string? RefreshToken { get; set; }
            public int ExpiresIn { get; set; }
            public string TokenType { get; set; } = string.Empty;
            public string? Scope { get; set; }
        }
    }
}
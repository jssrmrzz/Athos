using Athos.ReviewAutomation.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Athos.ReviewAutomation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OAuthController : ControllerBase
    {
        private readonly IGoogleOAuthService _googleOAuthService;
        private readonly IBusinessContextService _businessContextService;
        private readonly ILogger<OAuthController> _logger;

        public OAuthController(
            IGoogleOAuthService googleOAuthService,
            IBusinessContextService businessContextService,
            ILogger<OAuthController> logger)
        {
            _googleOAuthService = googleOAuthService;
            _businessContextService = businessContextService;
            _logger = logger;
        }

        [HttpGet("google/authorize")]
        public IActionResult AuthorizeGoogle()
        {
            _logger.LogInformation("OAuth authorize endpoint called");
            
            var businessId = _businessContextService.GetCurrentBusinessId();
            if (businessId == null)
            {
                _logger.LogWarning("Business context not found in OAuth authorize request");
                return BadRequest("Business context not found");
            }

            _logger.LogInformation("Generating OAuth authorization URL for business {BusinessId}", businessId);

            try
            {
                var authUrl = _googleOAuthService.GetAuthorizationUrl(businessId.Value);
                _logger.LogInformation("Successfully generated OAuth authorization URL for business {BusinessId}", businessId);
                return Ok(new { authorizationUrl = authUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate authorization URL for business {BusinessId}", businessId);
                return StatusCode(500, "Failed to generate authorization URL");
            }
        }

        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string state, [FromQuery] string? error)
        {
            _logger.LogInformation("OAuth callback received - Code: {CodePresent}, State: {State}, Error: {Error}", 
                !string.IsNullOrEmpty(code), state, error);

            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogWarning("OAuth callback received error: {Error}", error);
                return BadRequest($"OAuth error: {error}");
            }

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
            {
                _logger.LogError("OAuth callback missing required parameters - Code: {CodePresent}, State: {StatePresent}", 
                    !string.IsNullOrEmpty(code), !string.IsNullOrEmpty(state));
                return BadRequest("Missing code or state parameter");
            }

            try
            {
                // Extract business ID from state parameter
                var businessId = ExtractBusinessIdFromState(state);
                _logger.LogInformation("Extracted business ID {BusinessId} from state parameter {State}", businessId, state);
                
                if (businessId == null)
                {
                    _logger.LogError("Failed to extract business ID from state parameter: {State}", state);
                    return BadRequest("Invalid state parameter");
                }

                _logger.LogInformation("Starting token exchange for business {BusinessId} with authorization code", businessId.Value);
                var token = await _googleOAuthService.ExchangeCodeForTokenAsync(businessId.Value, code);
                
                _logger.LogInformation("Successfully authenticated business {BusinessId} with Google. Token expires at: {ExpiresAt}", 
                    businessId.Value, token.ExpiresAt);
                
                // Redirect to frontend with success message
                return Redirect($"http://localhost:5173/business/settings?oauth=success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to exchange OAuth code for token. Code length: {CodeLength}, State: {State}", 
                    code?.Length ?? 0, state);
                return Redirect($"http://localhost:5173/business/settings?oauth=error");
            }
        }

        [HttpPost("google/refresh")]
        public async Task<IActionResult> RefreshGoogleToken()
        {
            var businessId = _businessContextService.GetCurrentBusinessId();
            if (businessId == null)
            {
                return BadRequest("Business context not found");
            }

            try
            {
                var token = await _googleOAuthService.RefreshTokenAsync(businessId.Value);
                return Ok(new { 
                    success = true, 
                    expiresAt = token.ExpiresAt,
                    scope = token.Scope
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh token for business {BusinessId}", businessId);
                return StatusCode(500, "Failed to refresh token");
            }
        }

        [HttpPost("google/revoke")]
        public async Task<IActionResult> RevokeGoogleToken()
        {
            var businessId = _businessContextService.GetCurrentBusinessId();
            if (businessId == null)
            {
                return BadRequest("Business context not found");
            }

            try
            {
                var success = await _googleOAuthService.RevokeTokenAsync(businessId.Value);
                if (success)
                {
                    return Ok(new { success = true, message = "Token revoked successfully" });
                }
                else
                {
                    return BadRequest("Failed to revoke token");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to revoke token for business {BusinessId}", businessId);
                return StatusCode(500, "Failed to revoke token");
            }
        }

        [HttpGet("google/status")]
        public async Task<IActionResult> GetGoogleOAuthStatus()
        {
            var businessId = _businessContextService.GetCurrentBusinessId();
            if (businessId == null)
            {
                return BadRequest("Business context not found");
            }

            try
            {
                _logger.LogInformation("Checking OAuth status for business {BusinessId}", businessId.Value);
                var token = await _googleOAuthService.GetTokenAsync(businessId.Value);
                
                if (token == null)
                {
                    _logger.LogInformation("No OAuth token found for business {BusinessId}", businessId.Value);
                    return Ok(new { 
                        isConnected = false, 
                        hasToken = false 
                    });
                }

                _logger.LogInformation("Found OAuth token for business {BusinessId} - Valid: {IsValid}, Expired: {IsExpired}, Revoked: {IsRevoked}", 
                    businessId.Value, token.IsValid, token.IsExpired, token.IsRevoked);

                // Get user profile information if token is valid
                object userProfile = null;
                if (token.IsValid)
                {
                    _logger.LogInformation("Token is valid, fetching user profile for business {BusinessId}", businessId.Value);
                    var profile = await _googleOAuthService.GetUserProfileAsync(businessId.Value);
                    if (profile != null)
                    {
                        _logger.LogInformation("Successfully retrieved user profile for business {BusinessId}: {UserName} ({UserEmail})", 
                            businessId.Value, profile.Name, profile.Email);
                        userProfile = new
                        {
                            name = profile.Name,
                            email = profile.Email,
                            picture = profile.Picture,
                            givenName = profile.GivenName,
                            familyName = profile.FamilyName
                        };
                    }
                    else
                    {
                        _logger.LogWarning("Failed to retrieve user profile for business {BusinessId} despite valid token", businessId.Value);
                    }
                }

                return Ok(new { 
                    isConnected = token.IsValid,
                    hasToken = true,
                    expiresAt = token.ExpiresAt,
                    scope = token.Scope,
                    isExpired = token.IsExpired,
                    isRevoked = token.IsRevoked,
                    userProfile = userProfile
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get OAuth status for business {BusinessId}", businessId);
                return StatusCode(500, "Failed to get OAuth status");
            }
        }

        private int? ExtractBusinessIdFromState(string state)
        {
            try
            {
                // State format: "businessId" or "businessId:additionalData"
                var parts = state.Split(':');
                if (int.TryParse(parts[0], out int businessId))
                {
                    return businessId;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
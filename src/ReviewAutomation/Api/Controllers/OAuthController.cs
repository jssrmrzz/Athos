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
            var businessId = _businessContextService.GetCurrentBusinessId();
            if (businessId == null)
            {
                return BadRequest("Business context not found");
            }

            try
            {
                var authUrl = _googleOAuthService.GetAuthorizationUrl(businessId.Value);
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
            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogWarning("OAuth callback received error: {Error}", error);
                return BadRequest($"OAuth error: {error}");
            }

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
            {
                return BadRequest("Missing code or state parameter");
            }

            try
            {
                // Extract business ID from state parameter
                var businessId = ExtractBusinessIdFromState(state);
                if (businessId == null)
                {
                    return BadRequest("Invalid state parameter");
                }

                var token = await _googleOAuthService.ExchangeCodeForTokenAsync(businessId.Value, code);
                
                _logger.LogInformation("Successfully authenticated business {BusinessId} with Google", businessId);
                
                // Redirect to frontend with success message
                return Redirect($"http://localhost:5173/business/settings?oauth=success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to exchange OAuth code for token");
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
                var token = await _googleOAuthService.GetTokenAsync(businessId.Value);
                if (token == null)
                {
                    return Ok(new { 
                        isConnected = false, 
                        hasToken = false 
                    });
                }

                return Ok(new { 
                    isConnected = token.IsValid,
                    hasToken = true,
                    expiresAt = token.ExpiresAt,
                    scope = token.Scope,
                    isExpired = token.IsExpired,
                    isRevoked = token.IsRevoked
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
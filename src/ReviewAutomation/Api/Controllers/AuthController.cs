using Athos.ReviewAutomation.Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Athos.ReviewAutomation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IGoogleOAuthService _googleOAuthService;
        private readonly IBusinessContextService _businessContextService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IGoogleOAuthService googleOAuthService,
            IBusinessContextService businessContextService,
            ILogger<AuthController> logger)
        {
            _googleOAuthService = googleOAuthService;
            _businessContextService = businessContextService;
            _logger = logger;
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                _logger.LogInformation("Logout endpoint called");

                // Get current business context if available
                var businessId = _businessContextService.GetCurrentBusinessId();
                if (businessId.HasValue)
                {
                    _logger.LogInformation("Revoking OAuth tokens for business {BusinessId} during logout", businessId.Value);
                    
                    try
                    {
                        // Revoke OAuth tokens for the current business
                        await _googleOAuthService.RevokeTokenAsync(businessId.Value);
                        _logger.LogInformation("Successfully revoked OAuth tokens for business {BusinessId}", businessId.Value);
                    }
                    catch (Exception ex)
                    {
                        // Log the error but don't fail the logout process
                        _logger.LogWarning(ex, "Failed to revoke OAuth tokens for business {BusinessId} during logout", businessId.Value);
                    }
                }

                // Sign out from ASP.NET Core authentication
                await HttpContext.SignOutAsync();
                
                _logger.LogInformation("User successfully logged out");

                return Ok(new { 
                    success = true, 
                    message = "Successfully logged out",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout process");
                
                // Even if there's an error, we should try to sign out
                try
                {
                    await HttpContext.SignOutAsync();
                }
                catch (Exception signOutEx)
                {
                    _logger.LogError(signOutEx, "Failed to sign out during error handling");
                }

                return StatusCode(500, new { 
                    success = false, 
                    message = "Logout completed with errors",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("status")]
        public IActionResult GetAuthStatus()
        {
            try
            {
                var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
                var businessId = _businessContextService.GetCurrentBusinessId();

                return Ok(new
                {
                    isAuthenticated = isAuthenticated,
                    businessId = businessId,
                    userName = User.Identity?.Name,
                    authType = User.Identity?.AuthenticationType,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting authentication status");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Failed to get authentication status" 
                });
            }
        }
    }
}
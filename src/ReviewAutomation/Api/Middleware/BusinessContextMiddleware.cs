using Athos.ReviewAutomation.Core.Interfaces;
using System.Security.Claims;

namespace Athos.ReviewAutomation.Api.Middleware
{
    public class BusinessContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BusinessContextMiddleware> _logger;

        public BusinessContextMiddleware(RequestDelegate next, ILogger<BusinessContextMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IBusinessContextService businessContextService)
        {
            // Skip middleware for health checks, auth endpoints, and mock APIs
            if (context.Request.Path.StartsWithSegments("/api/health") ||
                context.Request.Path.StartsWithSegments("/api/auth") ||
                context.Request.Path.StartsWithSegments("/api/mock") ||
                context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            // For OAuth endpoints, allow business context from headers or query parameters even without authentication
            if (context.Request.Path.StartsWithSegments("/api/oauth"))
            {
                var businessIdFromHeader = context.Request.Headers["X-Business-Id"].FirstOrDefault();
                var businessIdFromQuery = context.Request.Query["businessId"].FirstOrDefault();
                var businessIdString = businessIdFromHeader ?? businessIdFromQuery;
                
                if (!string.IsNullOrEmpty(businessIdString) && int.TryParse(businessIdString, out var businessId))
                {
                    // Create a mock identity for OAuth endpoints
                    var identity = new ClaimsIdentity();
                    identity.AddClaim(new Claim("BusinessId", businessId.ToString()));
                    identity.AddClaim(new Claim("UserId", "1"));
                    identity.AddClaim(new Claim("Role", "Owner"));
                    
                    context.User = new ClaimsPrincipal(identity);
                    businessContextService.SetBusinessContext(businessId, 1, "Owner");
                }
                await _next(context);
                return;
            }

            // Skip if not authenticated for other endpoints
            if (!context.User.Identity?.IsAuthenticated == true)
            {
                await _next(context);
                return;
            }

            try
            {
                // Extract business context from query params or headers for business switching
                var businessIdFromQuery = context.Request.Query["businessId"].FirstOrDefault();
                var businessIdFromHeader = context.Request.Headers["X-Business-Id"].FirstOrDefault();
                
                var requestedBusinessId = businessIdFromQuery ?? businessIdFromHeader;

                if (!string.IsNullOrEmpty(requestedBusinessId) && int.TryParse(requestedBusinessId, out var businessId))
                {
                    // Validate user has access to this business
                    var userId = GetUserIdFromClaims(context.User);
                    if (userId.HasValue)
                    {
                        // This would typically check if the user has access to the business
                        // For now, we'll set the context and let the business logic handle validation
                        var businessUser = await GetBusinessUserAsync(businessContextService, businessId, userId.Value);
                        if (businessUser != null)
                        {
                            businessContextService.SetBusinessContext(businessId, userId.Value, businessUser.Role);
                        }
                        else
                        {
                            _logger.LogWarning("User {UserId} attempted to access business {BusinessId} without permission", userId, businessId);
                            context.Response.StatusCode = 403;
                            await context.Response.WriteAsync("Access denied to the requested business");
                            return;
                        }
                    }
                }

                // Ensure user has a business context set
                var currentBusinessId = businessContextService.GetCurrentBusinessId();
                if (!currentBusinessId.HasValue)
                {
                    // If no business context is set, try to set to user's first business
                    var userId = GetUserIdFromClaims(context.User);
                    if (userId.HasValue)
                    {
                        var userBusinesses = await GetUserBusinessesAsync(businessContextService, userId.Value);
                        if (userBusinesses.Any())
                        {
                            var firstBusiness = userBusinesses.First();
                            businessContextService.SetBusinessContext(firstBusiness.BusinessId, userId.Value, firstBusiness.Role);
                        }
                        else
                        {
                            _logger.LogWarning("User {UserId} has no business associations", userId);
                            context.Response.StatusCode = 403;
                            await context.Response.WriteAsync("No business access configured");
                            return;
                        }
                    }
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BusinessContextMiddleware");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal server error");
            }
        }

        private static int? GetUserIdFromClaims(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst("UserId")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private static async Task<BusinessUserInfo?> GetBusinessUserAsync(IBusinessContextService businessContextService, int businessId, int userId)
        {
            // This is a simplified implementation - in a real scenario you'd inject IBusinessRepository
            // For now, we'll use the business context service
            try
            {
                var businessUser = await businessContextService.GetCurrentBusinessUserAsync();
                return businessUser != null ? new BusinessUserInfo { BusinessId = businessUser.BusinessId, UserId = businessUser.UserId, Role = businessUser.Role } : null;
            }
            catch
            {
                return null;
            }
        }

        private static async Task<List<BusinessUserInfo>> GetUserBusinessesAsync(IBusinessContextService businessContextService, int userId)
        {
            // This is a simplified implementation - you'd typically inject IBusinessRepository for this
            // For now, return empty list to let the application handle this in the business logic layer
            return new List<BusinessUserInfo>();
        }

        private class BusinessUserInfo
        {
            public int BusinessId { get; set; }
            public int UserId { get; set; }
            public string Role { get; set; } = string.Empty;
        }
    }
}
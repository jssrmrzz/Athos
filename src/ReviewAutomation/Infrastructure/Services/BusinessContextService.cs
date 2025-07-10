using Athos.ReviewAutomation.Core.Entities;
using Athos.ReviewAutomation.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Athos.ReviewAutomation.Infrastructure.Services
{
    public class BusinessContextService : IBusinessContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBusinessRepository _businessRepository;

        public BusinessContextService(IHttpContextAccessor httpContextAccessor, IBusinessRepository businessRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _businessRepository = businessRepository;
        }

        public int? GetCurrentBusinessId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity == null)
                return null;

            var businessIdClaim = httpContext.User.FindFirst("BusinessId")?.Value;
            if (int.TryParse(businessIdClaim, out var businessId))
                return businessId;

            return null;
        }

        public async Task<Business?> GetCurrentBusinessAsync()
        {
            var businessId = GetCurrentBusinessId();
            if (!businessId.HasValue)
                return null;

            return await _businessRepository.GetByIdAsync(businessId.Value);
        }

        public async Task<BusinessUser?> GetCurrentBusinessUserAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return null;

            var businessId = GetCurrentBusinessId();
            var userIdClaim = httpContext.User.FindFirst("UserId")?.Value;

            if (!businessId.HasValue || !int.TryParse(userIdClaim, out var userId))
                return null;

            return await _businessRepository.GetBusinessUserAsync(businessId.Value, userId);
        }

        public bool HasBusinessAccess(int businessId)
        {
            var currentBusinessId = GetCurrentBusinessId();
            return currentBusinessId.HasValue && currentBusinessId.Value == businessId;
        }

        public bool HasRole(string requiredRole)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return false;

            var userRole = httpContext.User.FindFirst("Role")?.Value;
            if (string.IsNullOrEmpty(userRole))
                return false;

            return string.Equals(userRole, requiredRole, StringComparison.OrdinalIgnoreCase);
        }

        public bool HasPermission(string requiredRole)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return false;

            var userRole = httpContext.User.FindFirst("Role")?.Value;
            if (string.IsNullOrEmpty(userRole))
                return false;

            return BusinessUserRoles.HasPermission(userRole, requiredRole);
        }

        public void SetBusinessContext(int businessId, int userId, string role)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity is ClaimsIdentity identity)
            {
                // Remove existing business context claims
                var existingClaims = identity.Claims.Where(c => 
                    c.Type == "BusinessId" || 
                    c.Type == "UserId" || 
                    c.Type == "Role").ToList();
                
                foreach (var claim in existingClaims)
                {
                    identity.RemoveClaim(claim);
                }

                // Add new business context claims
                identity.AddClaim(new Claim("BusinessId", businessId.ToString()));
                identity.AddClaim(new Claim("UserId", userId.ToString()));
                identity.AddClaim(new Claim("Role", role));
            }
        }

        public void ClearBusinessContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity is ClaimsIdentity identity)
            {
                var businessClaims = identity.Claims.Where(c => 
                    c.Type == "BusinessId" || 
                    c.Type == "UserId" || 
                    c.Type == "Role").ToList();
                
                foreach (var claim in businessClaims)
                {
                    identity.RemoveClaim(claim);
                }
            }
        }
    }
}
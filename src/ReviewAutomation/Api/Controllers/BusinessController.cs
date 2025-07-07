using Athos.ReviewAutomation.Core.Entities;
using Athos.ReviewAutomation.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Athos.ReviewAutomation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IBusinessContextService _businessContext;
        private readonly ILogger<BusinessController> _logger;

        public BusinessController(
            IBusinessRepository businessRepository,
            IBusinessContextService businessContext,
            ILogger<BusinessController> logger)
        {
            _businessRepository = businessRepository;
            _businessContext = businessContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<object>>> GetUserBusinesses()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized("User not found");

            var businesses = await _businessRepository.GetUserBusinessesAsync(userId.Value);
            
            var result = businesses.Select(b => new
            {
                b.Id,
                b.Name,
                b.Description,
                b.GoogleBusinessProfileId,
                b.SubscriptionTier,
                b.CreatedAt,
                UserRole = b.BusinessUsers.FirstOrDefault(bu => bu.UserId == userId.Value)?.Role ?? "Viewer"
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetBusiness(int id)
        {
            if (!_businessContext.HasBusinessAccess(id))
                return Forbid("Access denied to this business");

            var business = await _businessRepository.GetByIdAsync(id);
            if (business == null)
                return NotFound("Business not found");

            var result = new
            {
                business.Id,
                business.Name,
                business.Description,
                business.GoogleBusinessProfileId,
                business.SubscriptionTier,
                business.CreatedAt,
                business.UpdatedAt,
                Settings = business.Settings != null ? new
                {
                    business.Settings.PreferredLlmProvider,
                    business.Settings.DefaultResponseTemplate,
                    business.Settings.AutoRespondToPositiveReviews,
                    business.Settings.EnableEmailNotifications,
                    business.Settings.EnableSmsNotifications,
                    business.Settings.SmsPhoneNumber,
                    business.Settings.ReviewSyncIntervalMinutes
                } : null
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateBusiness([FromBody] CreateBusinessRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized("User not found");

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Business name is required");

            var business = new Business
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                GoogleBusinessProfileId = request.GoogleBusinessProfileId?.Trim(),
                SubscriptionTier = "Free"
            };

            var createdBusiness = await _businessRepository.CreateAsync(business);

            // Add the creator as the owner
            var businessUser = new BusinessUser
            {
                BusinessId = createdBusiness.Id,
                UserId = userId.Value,
                Role = BusinessUserRoles.Owner,
                InvitedByUserId = userId.Value
            };

            await _businessRepository.AddUserToBusinessAsync(businessUser);

            _logger.LogInformation("User {UserId} created business {BusinessId}: {BusinessName}", 
                userId, createdBusiness.Id, createdBusiness.Name);

            return CreatedAtAction(nameof(GetBusiness), new { id = createdBusiness.Id }, new
            {
                createdBusiness.Id,
                createdBusiness.Name,
                createdBusiness.Description,
                createdBusiness.GoogleBusinessProfileId,
                createdBusiness.SubscriptionTier,
                UserRole = BusinessUserRoles.Owner
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateBusiness(int id, [FromBody] UpdateBusinessRequest request)
        {
            if (!_businessContext.HasBusinessAccess(id))
                return Forbid("Access denied to this business");

            if (!_businessContext.HasPermission(BusinessUserRoles.Admin))
                return Forbid("Admin access required");

            var business = await _businessRepository.GetByIdAsync(id);
            if (business == null)
                return NotFound("Business not found");

            if (!string.IsNullOrWhiteSpace(request.Name))
                business.Name = request.Name.Trim();
            
            if (request.Description != null)
                business.Description = request.Description.Trim();
            
            if (request.GoogleBusinessProfileId != null)
                business.GoogleBusinessProfileId = request.GoogleBusinessProfileId.Trim();

            await _businessRepository.UpdateAsync(business);

            _logger.LogInformation("Business {BusinessId} updated by user {UserId}", id, GetCurrentUserId());

            return Ok(new
            {
                business.Id,
                business.Name,
                business.Description,
                business.GoogleBusinessProfileId,
                business.SubscriptionTier,
                business.UpdatedAt
            });
        }

        [HttpGet("{id}/members")]
        public async Task<ActionResult<List<object>>> GetBusinessMembers(int id)
        {
            if (!_businessContext.HasBusinessAccess(id))
                return Forbid("Access denied to this business");

            var members = await _businessRepository.GetBusinessMembersAsync(id);
            
            var result = members.Select(m => new
            {
                m.Id,
                m.Role,
                m.CreatedAt,
                m.LastAccessAt,
                User = new
                {
                    m.User.Id,
                    m.User.Name,
                    m.User.Email,
                    m.User.ProfilePictureUrl
                },
                InvitedBy = m.InvitedBy != null ? new
                {
                    m.InvitedBy.Id,
                    m.InvitedBy.Name
                } : null
            });

            return Ok(result);
        }

        [HttpPost("{id}/members")]
        public async Task<ActionResult> InviteUser(int id, [FromBody] InviteUserRequest request)
        {
            if (!_businessContext.HasBusinessAccess(id))
                return Forbid("Access denied to this business");

            if (!_businessContext.HasPermission(BusinessUserRoles.Admin))
                return Forbid("Admin access required");

            // This is a simplified implementation - in reality you'd:
            // 1. Create an invitation record
            // 2. Send an email invitation
            // 3. Handle the invitation acceptance flow
            
            return Ok(new { message = "User invitation feature not yet implemented" });
        }

        [HttpPut("{id}/members/{userId}/role")]
        public async Task<ActionResult> UpdateMemberRole(int id, int userId, [FromBody] UpdateRoleRequest request)
        {
            if (!_businessContext.HasBusinessAccess(id))
                return Forbid("Access denied to this business");

            if (!_businessContext.HasPermission(BusinessUserRoles.Admin))
                return Forbid("Admin access required");

            if (!BusinessUserRoles.IsValidRole(request.Role))
                return BadRequest("Invalid role specified");

            await _businessRepository.UpdateBusinessUserRoleAsync(id, userId, request.Role);

            _logger.LogInformation("User {UserId} role updated to {Role} in business {BusinessId}", 
                userId, request.Role, id);

            return Ok();
        }

        [HttpDelete("{id}/members/{userId}")]
        public async Task<ActionResult> RemoveMember(int id, int userId)
        {
            if (!_businessContext.HasBusinessAccess(id))
                return Forbid("Access denied to this business");

            if (!_businessContext.HasPermission(BusinessUserRoles.Admin))
                return Forbid("Admin access required");

            var currentUserId = GetCurrentUserId();
            if (currentUserId == userId)
                return BadRequest("Cannot remove yourself from the business");

            await _businessRepository.RemoveUserFromBusinessAsync(id, userId);

            _logger.LogInformation("User {UserId} removed from business {BusinessId}", userId, id);

            return Ok();
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public class CreateBusinessRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? GoogleBusinessProfileId { get; set; }
    }

    public class UpdateBusinessRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? GoogleBusinessProfileId { get; set; }
    }

    public class InviteUserRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = BusinessUserRoles.Viewer;
    }

    public class UpdateRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }
}
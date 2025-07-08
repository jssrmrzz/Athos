using Athos.ReviewAutomation.Core.Entities;
using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Athos.ReviewAutomation.Infrastructure.Repositories
{
    public class BusinessRepository : IBusinessRepository
    {
        private readonly ReviewDbContext _context;

        public BusinessRepository(ReviewDbContext context)
        {
            _context = context;
        }

        public async Task<Business?> GetByIdAsync(int id)
        {
            return await _context.Businesses
                .Include(b => b.Settings)
                .Include(b => b.BusinessUsers)
                    .ThenInclude(bu => bu.User)
                .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
        }

        public async Task<Business?> GetByGoogleProfileIdAsync(string googleProfileId)
        {
            return await _context.Businesses
                .Include(b => b.Settings)
                .FirstOrDefaultAsync(b => b.GoogleBusinessProfileId == googleProfileId && b.IsActive);
        }

        public async Task<List<Business>> GetUserBusinessesAsync(int userId)
        {
            return await _context.BusinessUsers
                .Where(bu => bu.UserId == userId && bu.IsActive)
                .Include(bu => bu.Business)
                    .ThenInclude(b => b.Settings)
                .Select(bu => bu.Business)
                .Where(b => b.IsActive)
                .ToListAsync();
        }

        public async Task<Business> CreateAsync(Business business)
        {
            business.CreatedAt = DateTime.UtcNow;
            business.UpdatedAt = DateTime.UtcNow;
            
            _context.Businesses.Add(business);
            await _context.SaveChangesAsync();
            return business;
        }

        public async Task<Business> UpdateAsync(Business business)
        {
            business.UpdatedAt = DateTime.UtcNow;
            _context.Businesses.Update(business);
            await _context.SaveChangesAsync();
            return business;
        }

        public async Task DeleteAsync(int id)
        {
            var business = await _context.Businesses.FindAsync(id);
            if (business != null)
            {
                business.IsActive = false;
                business.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Businesses.AnyAsync(b => b.Id == id && b.IsActive);
        }

        public async Task<BusinessUser?> GetBusinessUserAsync(int businessId, int userId)
        {
            return await _context.BusinessUsers
                .Include(bu => bu.Business)
                .Include(bu => bu.User)
                .FirstOrDefaultAsync(bu => bu.BusinessId == businessId && bu.UserId == userId && bu.IsActive);
        }

        public async Task<BusinessUser> AddUserToBusinessAsync(BusinessUser businessUser)
        {
            businessUser.CreatedAt = DateTime.UtcNow;
            businessUser.IsActive = true;
            
            _context.BusinessUsers.Add(businessUser);
            await _context.SaveChangesAsync();
            return businessUser;
        }

        public async Task<BusinessUser> UpdateBusinessUserRoleAsync(int businessId, int userId, string role)
        {
            var businessUser = await _context.BusinessUsers
                .FirstOrDefaultAsync(bu => bu.BusinessId == businessId && bu.UserId == userId && bu.IsActive);
            
            if (businessUser == null)
                throw new InvalidOperationException("Business user relationship not found");

            businessUser.Role = role;
            await _context.SaveChangesAsync();
            return businessUser;
        }

        public async Task RemoveUserFromBusinessAsync(int businessId, int userId)
        {
            var businessUser = await _context.BusinessUsers
                .FirstOrDefaultAsync(bu => bu.BusinessId == businessId && bu.UserId == userId);
            
            if (businessUser != null)
            {
                businessUser.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<BusinessUser>> GetBusinessMembersAsync(int businessId)
        {
            return await _context.BusinessUsers
                .Where(bu => bu.BusinessId == businessId && bu.IsActive)
                .Include(bu => bu.User)
                .Include(bu => bu.InvitedBy)
                .OrderBy(bu => bu.Role)
                .ThenBy(bu => bu.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        
        public async Task<BusinessOAuthToken?> GetOAuthTokenAsync(int businessId, string provider = "Google")
        {
            return await _context.BusinessOAuthTokens
                .FirstOrDefaultAsync(t => t.BusinessId == businessId && t.Provider == provider);
        }

        public async Task<bool> HasValidOAuthTokenAsync(int businessId, string provider = "Google")
        {
            return await _context.BusinessOAuthTokens
                .AnyAsync(t => t.BusinessId == businessId && t.Provider == provider && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);
        }
    }
}
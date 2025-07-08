using Athos.ReviewAutomation.Core.Entities;

namespace Athos.ReviewAutomation.Core.Interfaces
{
    public interface IBusinessRepository
    {
        Task<Business?> GetByIdAsync(int id);
        Task<Business?> GetByGoogleProfileIdAsync(string googleProfileId);
        Task<List<Business>> GetUserBusinessesAsync(int userId);
        Task<Business> CreateAsync(Business business);
        Task<Business> UpdateAsync(Business business);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<BusinessUser?> GetBusinessUserAsync(int businessId, int userId);
        Task<BusinessUser> AddUserToBusinessAsync(BusinessUser businessUser);
        Task<BusinessUser> UpdateBusinessUserRoleAsync(int businessId, int userId, string role);
        Task RemoveUserFromBusinessAsync(int businessId, int userId);
        Task<List<BusinessUser>> GetBusinessMembersAsync(int businessId);
        Task SaveChangesAsync();
        
        // OAuth token methods
        Task<BusinessOAuthToken?> GetOAuthTokenAsync(int businessId, string provider = "Google");
        Task<bool> HasValidOAuthTokenAsync(int businessId, string provider = "Google");
    }
}
using Athos.ReviewAutomation.Core.Entities;

namespace Athos.ReviewAutomation.Core.Interfaces
{
    public interface IBusinessContextService
    {
        int? GetCurrentBusinessId();
        Task<Business?> GetCurrentBusinessAsync();
        Task<BusinessUser?> GetCurrentBusinessUserAsync();
        bool HasBusinessAccess(int businessId);
        bool HasRole(string requiredRole);
        bool HasPermission(string requiredRole);
        void SetBusinessContext(int businessId, int userId, string role);
        void ClearBusinessContext();
    }
}
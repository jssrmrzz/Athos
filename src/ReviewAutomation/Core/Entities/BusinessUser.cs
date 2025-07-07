using System.ComponentModel.DataAnnotations;

namespace Athos.ReviewAutomation.Core.Entities
{
    public class BusinessUser
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int BusinessId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Viewer";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastAccessAt { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public int? InvitedByUserId { get; set; }
        
        // Navigation properties
        public Business Business { get; set; } = null!;
        public User User { get; set; } = null!;
        public User? InvitedBy { get; set; }
    }
    
    public static class BusinessUserRoles
    {
        public const string Owner = "Owner";
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Viewer = "Viewer";
        
        public static readonly string[] AllRoles = { Owner, Admin, Manager, Viewer };
        
        public static bool IsValidRole(string role)
        {
            return AllRoles.Contains(role);
        }
        
        public static bool HasPermission(string userRole, string requiredRole)
        {
            var userRoleLevel = GetRoleLevel(userRole);
            var requiredRoleLevel = GetRoleLevel(requiredRole);
            return userRoleLevel >= requiredRoleLevel;
        }
        
        private static int GetRoleLevel(string role)
        {
            return role switch
            {
                Owner => 4,
                Admin => 3,
                Manager => 2,
                Viewer => 1,
                _ => 0
            };
        }
    }
}
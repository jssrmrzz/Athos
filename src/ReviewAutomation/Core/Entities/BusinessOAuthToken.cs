using System.ComponentModel.DataAnnotations;

namespace Athos.ReviewAutomation.Core.Entities
{
    public class BusinessOAuthToken
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int BusinessId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Provider { get; set; } = "Google";
        
        [Required]
        public string AccessToken { get; set; } = string.Empty;
        
        public string? RefreshToken { get; set; }
        
        public DateTime ExpiresAt { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsRevoked { get; set; } = false;
        
        [MaxLength(500)]
        public string? Scope { get; set; }
        
        // Navigation properties
        public Business Business { get; set; } = null!;
        
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        
        public bool IsValid => !IsRevoked && !IsExpired;
    }
}
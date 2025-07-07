using System.ComponentModel.DataAnnotations;

namespace Athos.ReviewAutomation.Core.Entities
{
    public class Business
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [MaxLength(100)]
        public string? GoogleBusinessProfileId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string SubscriptionTier { get; set; } = "Free";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public ICollection<BusinessUser> BusinessUsers { get; set; } = new List<BusinessUser>();
        public ICollection<DbReview> Reviews { get; set; } = new List<DbReview>();
        public BusinessSettings? Settings { get; set; }
        public ICollection<BusinessOAuthToken> OAuthTokens { get; set; } = new List<BusinessOAuthToken>();
    }
}
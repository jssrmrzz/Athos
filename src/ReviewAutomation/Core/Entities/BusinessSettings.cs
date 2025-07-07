using System.ComponentModel.DataAnnotations;

namespace Athos.ReviewAutomation.Core.Entities
{
    public class BusinessSettings
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int BusinessId { get; set; }
        
        [MaxLength(50)]
        public string? PreferredLlmProvider { get; set; } = "Local";
        
        [MaxLength(1000)]
        public string? DefaultResponseTemplate { get; set; }
        
        public bool AutoRespondToPositiveReviews { get; set; } = false;
        
        public bool EnableEmailNotifications { get; set; } = true;
        
        public bool EnableSmsNotifications { get; set; } = false;
        
        [MaxLength(20)]
        public string? SmsPhoneNumber { get; set; }
        
        public int ReviewSyncIntervalMinutes { get; set; } = 60;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Business Business { get; set; } = null!;
    }
}
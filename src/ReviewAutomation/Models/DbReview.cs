using System.ComponentModel.DataAnnotations;

namespace Athos.ReviewAutomation.Models
{
    public class DbReview
{
    [Key]
    public string? ReviewId { get; set; }
    public string? Author { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime Timestamp { get; set; }

    public string Sentiment { get; set; } = "Unclassified";
    public string? SuggestedResponse { get; set; }

    public bool AlertSent { get; set; } = false;

    public string? FinalResponse { get; set; }
    public bool IsApproved { get; set; } = false;
    
    public DateTime? ApprovedAt { get; set; }


    public string Status
    {
        get
        {
            if (IsApproved) return "Responded";
            if (Sentiment == "Positive" && SuggestedResponse != null) return "Pending Approval";
            if (Sentiment == "Negative" && AlertSent) return "Needs Attention";
            return "Unclassified";
        }
    }
    
    public string SubmittedAgo
    {
        get
        {
            var timeSpan = DateTime.UtcNow - Timestamp;

            if (timeSpan.TotalMinutes < 1)
                return "just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minute(s) ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hour(s) ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} day(s) ago";

            return Timestamp.ToString("MMM dd, yyyy");
        }
    }

    public string? ApprovedAgo
    {
        get
        {
            if (!ApprovedAt.HasValue) return null;

            var timeSpan = DateTime.UtcNow - ApprovedAt.Value;

            if (timeSpan.TotalMinutes < 1)
                return "just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minute(s) ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hour(s) ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} day(s) ago";

            return ApprovedAt.Value.ToString("MMM dd, yyyy");
        }
    }
}

}

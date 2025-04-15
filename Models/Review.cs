namespace Athos.Api.Models
{
    public class Review
{
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
}

}

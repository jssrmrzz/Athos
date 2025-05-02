namespace Athos.ReviewAutomation.Models
{
    public class ReviewOutputDto
    {
        public string ReviewId { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Sentiment { get; set; } = "Unclassified";
        public string Status { get; set; } = "Unclassified";
        public string? SuggestedResponse { get; set; }
        public string? FinalResponse { get; set; }
        public string SubmittedAgo { get; set; } = string.Empty;
        public string? ApprovedAgo { get; set; }
        public bool IsApproved { get; set; }
    }
}
namespace Athos.ReviewAutomation.Application.DTOs.Reviews
{
    public class GetReviewsResponseOutputDto
    {
        public string ReviewId { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Sentiment { get; set; } = "Unclassified";
        public string Status { get; set; } = "Unclassified";
        public string? SuggestedResponse { get; set; }
        public string? FinalResponse { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public bool IsApproved { get; set; }
    }
}
namespace Athos.ReviewAutomation.Api.Models
{
    public class SuggestionRequestDto
    {
        public string ReviewId { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
    }
}
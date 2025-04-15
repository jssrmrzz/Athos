namespace Athos.Api.Services
{
    public class AutoReplyService
    {
        public string? GenerateReply(string sentiment, string? authorName)
        {
            if (sentiment != "Positive") return null;

            return $"Thanks for the kind words{(string.IsNullOrWhiteSpace(authorName) ? "!" : $", {authorName}!")}";
        }
    }
}

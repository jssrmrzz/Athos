namespace Athos.Api.Services
{
    public class SentimentService
    {
        public string AnalyzeSentiment(int rating, string? comment)
        {
            if (rating >= 4)
                return "Positive";
            if (rating == 3)
                return "Neutral";
            return "Negative";
        }
    }
}

namespace Athos.ReviewAutomation.Models;
public class GoogleReviewDto
{
    public string ReviewId { get; set; }
    public string Name { get; set; }
    public Reviewer Reviewer { get; set; }
    public string StarRating { get; set; }
    public string Comment { get; set; }
    public string CreateTime { get; set; }
    public string UpdateTime { get; set; }
    public ReviewReply ReviewReply { get; set; }
}

public class Reviewer
{
    public string DisplayName { get; set; }
}

public class ReviewReply
{
    public string Comment { get; set; }
    public string UpdateTime { get; set; }
}


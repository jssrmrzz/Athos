using Athos.ReviewAutomation.Core.Entities;
using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Infrastructure.Repositories;

namespace Athos.ReviewAutomation.Infrastructure.Services
{
    public class GoogleReviewIngestionService : IGoogleReviewIngestionService
    {
        private readonly GoogleReviewClient _client;
        private readonly IReviewRepository _repo;
        private readonly IBusinessContextService _businessContextService;

        public GoogleReviewIngestionService(GoogleReviewClient client, IReviewRepository repo, IBusinessContextService businessContextService)
        {
            _client = client;
            _repo = repo;
            _businessContextService = businessContextService;
        }

        public async Task<int> FetchAndSaveReviewsAsync()
        {
            var businessId = _businessContextService.GetCurrentBusinessId();
            if (businessId == null)
            {
                throw new InvalidOperationException("Business context is required for review ingestion");
            }

            var newReviews = await _client.FetchReviewsForBusinessAsync(businessId.Value);

            var existingIds = _repo.GetAllReviews().Select(r => r.ReviewId).ToHashSet();

            var toInsert = newReviews
                .Where(r => !existingIds.Contains(r.ReviewId))
                .Select(r => new DbReview
                {
                    ReviewId = r.ReviewId,
                    Author = r.Reviewer?.DisplayName ?? "Anonymous",
                    Rating = StarRatingToInt(r.StarRating),
                    Comment = r.Comment,
                    SubmittedAt = DateTime.Parse(r.CreateTime),
                    FinalResponse = r.ReviewReply?.Comment,
                    ApprovedAt = string.IsNullOrEmpty(r.ReviewReply?.UpdateTime)
                        ? null
                        : DateTime.Parse(r.ReviewReply.UpdateTime),
                    IsApproved = r.ReviewReply != null
                })
                .ToList();

            if (toInsert.Any())
            {
                _repo.AddReviews(toInsert);
                _repo.SaveChanges();
            }

            return toInsert.Count();
        }
        
        private int StarRatingToInt(string? starRating)
        {
            return starRating?.ToUpperInvariant() switch
            {
                "ONE" => 1,
                "TWO" => 2,
                "THREE" => 3,
                "FOUR" => 4,
                "FIVE" => 5,
                _ => 0
            };
        }


    }

}




using Amigo.Application.Abstraction;
using Amigo.Application.Abstraction.Services;
using Amigo.Application.Mapping;
using Amigo.Application.Specifications.TourSpecification.User;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.Domain.Enum;
using Amigo.Domain.Errors.BusinessErrors;
using Amigo.SharedKernal.DTOs.Tour;

namespace Amigo.Application.Services;

public class UserTourReviewService(
    IValidationService validationService,
    IUnitOfWork unitOfWork,
    ITourReviewEligibilityReader eligibilityReader) : IUserTourReviewService
{
    public async Task<Result<TourReviewEligibilityDto>> GetEligibilityAsync(string? userId, Guid tourId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Ok(new TourReviewEligibilityDto(false, "login_required"));

        var tourRepo = unitOfWork.GetRepository<Tour, Guid>();
        var tourExists = await tourRepo.AnyAsync(new TourExistsByIdSpecification(tourId));
        if (!tourExists)
            return Result.Fail(new NotFoundError("Tour not found."));

        var can = await eligibilityReader.CanUserWriteReviewAsync(userId, tourId);
        if (can)
            return Result.Ok(new TourReviewEligibilityDto(true, "ok"));

        return Result.Ok(new TourReviewEligibilityDto(false, "not_eligible"));
    }

    public async Task<Result<Guid>> SubmitReviewAsync(string userId, CreateUserTourReviewRequestDto request)
    {
        var validation = await validationService.ValidateAsync(request);
        if (!validation.IsSuccess)
            return validation;

        var tourRepo = unitOfWork.GetRepository<Tour, Guid>();
        var tourExists = await tourRepo.AnyAsync(new TourExistsByIdSpecification(request.TourId));
        if (!tourExists)
            return Result.Fail(new NotFoundError("Tour not found."));

  
        var listingLang = string.IsNullOrWhiteSpace(request.Language)
            ? SupportedLanguage.en
            : EnumsMapping.ToLanguageEnum(request.Language!);

        var reviewId = Guid.NewGuid();
        var review = new Review
        {
            Id = reviewId,
            TourId = request.TourId,
            UserId = userId,
            Rate = request.Rating,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            TravelWith = request.TravelWith,
        };

        var translation = new ReviewTranslation
        {
            Id = Guid.NewGuid(),
            ReviewId = reviewId,
            Comment = request.Comment.Trim(),
            Language = listingLang,
        };

        await unitOfWork.GetRepository<Review, Guid>().AddAsync(review);
        await unitOfWork.GetRepository<ReviewTranslation, Guid>().AddAsync(translation);

        if (request.ImageUrls is { Count: > 0 })
        {
            var imgs = request.ImageUrls
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Select(url => new ReviewImage
                {
                    Id = Guid.NewGuid(),
                    ReviewId = reviewId,
                    Image = url.Trim()
                })
                .ToList();
            if (imgs.Count > 0)
                await unitOfWork.GetRepository<ReviewImage, Guid>().AddRangeAsync(imgs);
        }

        try 
        {
            await unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            var msg = ex.InnerException?.Message ?? ex.Message;
            return Result.Fail($"Database Error: {msg}");
        }

        return Result.Ok(reviewId);
    }

    public async Task<Result<int>> MarkAsHelpfulAsync(Guid reviewId, string? userId, string? ipAddress = null)
    {
        var reviewRepo = unitOfWork.GetRepository<Review, Guid>();
        var voteRepo = unitOfWork.GetRepository<ReviewVote, Guid>();

        var review = await reviewRepo.GetByIdAsync(reviewId);
        if (review is null)
            return Result.Fail(new NotFoundError("Review not found."));

        // 2. Check if user/IP already voted
        var existingVote = await voteRepo.GetByIdAsync(new UserReviewVoteExistsSpecification(reviewId, userId, ipAddress));
        
        if (existingVote != null)
        {
            voteRepo.Remove(existingVote);
            review.HelpfulCount = Math.Max(0, review.HelpfulCount - 1);
        }
        else
        {
            var vote = new ReviewVote
            {
                Id = Guid.NewGuid(),
                ReviewId = reviewId,
                UserId = userId,
                IpAddress = ipAddress,
                VotedAt = DateTime.UtcNow
            };

            await voteRepo.AddAsync(vote);
            review.HelpfulCount++;
        }

        await unitOfWork.SaveChangesAsync();
        return Result.Ok(review.HelpfulCount);
    }

    public async Task<Result> UpdateReviewAsync(string userId, Guid reviewId, UpdateUserTourReviewRequestDto request)
    {
        var validation = await validationService.ValidateAsync(request);
        if (!validation.IsSuccess)
            return validation;

        var reviewRepo = unitOfWork.GetRepository<Review, Guid>();
        var spec = new ReviewByIdWithDetailsSpecification(reviewId);
        var review = await reviewRepo.GetByIdAsync(spec);

        if (review is null)
            return Result.Fail(new NotFoundError("Review not found."));

        if (review.UserId != userId)
            return Result.Fail(new ForbiddenError("You can only edit your own reviews."));

        review.Rate = request.Rating;
        review.TravelWith = request.TravelWith;

        var translation = review.Translations.FirstOrDefault();
        if (translation != null)
        {
            translation.Comment = request.Comment.Trim();
        }

        review.Images.Clear();
        if (request.ImageUrls is { Count: > 0 })
        {
            foreach (var url in request.ImageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
            {
                review.Images.Add(new ReviewImage
                {
                    Id = Guid.NewGuid(),
                    ReviewId = reviewId,
                    Image = url.Trim()
                });
            }
        }

        await unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteReviewAsync(string userId, Guid reviewId)
    {
        var reviewRepo = unitOfWork.GetRepository<Review, Guid>();
        var review = await reviewRepo.GetByIdAsync(reviewId);

        if (review is null)
            return Result.Fail(new NotFoundError("Review not found."));

        if (review.UserId != userId)
            return Result.Fail(new ForbiddenError("You can only delete your own reviews."));

        review.SetIsDeleted(true);
        await unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }
}


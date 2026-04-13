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

        var reviewed = await unitOfWork.GetRepository<Review, Guid>()
            .AnyAsync(new UserTourReviewExistsSpecification(userId, tourId));
        if (reviewed)
            return Result.Ok(new TourReviewEligibilityDto(false, "already_reviewed"));

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

        var can = await eligibilityReader.CanUserWriteReviewAsync(userId, request.TourId);
        if (!can)
            return Result.Fail(new ForbiddenError(
                "You can only leave a review after payment and once the tour date has passed (or your booking is marked completed)."));

        var listingLang = string.IsNullOrWhiteSpace(request.Language)
            ? Language.English
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

        await unitOfWork.SaveChangesAsync();

        return Result.Ok(reviewId);
    }
}

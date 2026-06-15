namespace Amigo.SharedKernal.DTOs.Tour;

public record UserTourPriceTierDto(
    Guid PriceId,
    string Label,
    decimal UnitAmount,
    bool IsFree,
    string UserTypeGroup);

public record UserTourSlotDto(
    Guid SlotId,
    string StartTime,
    string? EndTime,
    bool Available,
    int MaxCapacity,
    int RemainingCapacity
    );

public record UserTourScheduleDayDto(
    string Date,
    IReadOnlyList<UserTourSlotDto> Slots);

public record UserTourReviewItemDto(
    Guid ReviewId,
    decimal Rating,
    string? Comment,
    string? AuthorLabel,
    string Date,
    int HelpfulCount,
    string? TravelWith,
    bool VotedHelpful,
    bool IsOwner,
    IReadOnlyList<string> ImageUrls);

public record UserTourTravelerPhotoDto(
    string ImageUrl,
    string Date,
    string? AuthorLabel);


public record UserTourReviewDTO
    (

        IReadOnlyList<UserTourReviewItemDto>? RecentReviews,
        IReadOnlyList<UserTourTravelerPhotoDto>? TravelerPhotos

    );


public record ImageDTO(
    string url
    );

public record UserTourDetailDto(
    Guid TourId,
    string Title,
    string? Description,
    string? HeroImageUrl,
    IReadOnlyList<string> ImageUrls,

    decimal? AverageRating,
    int ReviewCount,
    bool FreeCancellation,
    bool IsWheelchairAvailable,
    bool IsPitsAllowed,
    string? OriginalPrice,
    string? FromPrice,
    int? DiscountPercent,
    string DurationDisplay,
    string? GuideLanguage,
    string TourSlug,
    string CurrencyCode,
    
    string? DestinationName,
    string? CountryName,

    string? MeetingPoint,
    
    IReadOnlyList<string> Included,
    IReadOnlyList<string> NotIncluded,

    List<GetCancellationResponseDTO>? CancellationPolicyDescription
    
    );
    public record GetCancellationResponseDTO
   (
      Guid Id,
      string CancelationPolicyType,
      TimeSpan CancellationBefore,
      decimal RefundPercentage
  );

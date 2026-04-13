namespace Amigo.SharedKernal.DTOs.Tour;

public record UserTourListItemDto(
    Guid TourId,
    string Title,
    string? Description,
    string? HeroImageUrl,
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
    string TourSlug);

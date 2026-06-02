namespace Amigo.SharedKernal.DTOs.Tour;

public record UserTrendingTourItemDto(
    Guid TourId,
    string Title,
    string? Description,
    string? HeroImageUrl,
    decimal? AverageRating,
    int ReviewCount,
    string FilteredCurrency,
    
    string? FromPrice,
    decimal ? Discount,
    string BaseCurrency,
    string? BaseAmount,
    string TourSlug,
    string DestinationSlug);

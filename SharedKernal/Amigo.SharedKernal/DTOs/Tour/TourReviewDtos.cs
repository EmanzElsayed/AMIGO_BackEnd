using Amigo.SharedKernal.DTOs.Images;

namespace Amigo.SharedKernal.DTOs.Tour;

public record TourReviewEligibilityDto(
    bool CanWriteReview,
    string Reason);

public record CreateUserTourReviewRequestDto(
    Guid TourId,
    decimal Rating,
    string Comment,
    string? Language,
    string? TravelWith,
    List<ImageUrlsForReviewRequestDTO>? ImageUrls);

public record UpdateUserTourReviewRequestDto(
    decimal? Rating,
    string? Comment,
    string? TravelWith,
    List<ImageUrlsForReviewRequestDTO>? ImageUrls);

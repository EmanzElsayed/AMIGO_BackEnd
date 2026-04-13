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
    List<string>? ImageUrls);

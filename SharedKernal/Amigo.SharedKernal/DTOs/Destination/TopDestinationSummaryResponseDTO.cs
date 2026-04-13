namespace Amigo.SharedKernal.DTOs.Destination;

public record TopDestinationSummaryResponseDTO(
    Guid DestinationId,
    string Name,
    string CountryCode,
    string? ImageUrl,
    int ActivityCount,
    int ReviewCount,
    int TravelerCount,
    double? AverageRating
);

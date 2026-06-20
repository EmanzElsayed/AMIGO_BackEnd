using System;

namespace Amigo.Domain.DTO.Favorite;

public class FavoriteResponseDTO
{
    public Guid TourId { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid DestinationId { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? DestinationSlug {  get; set; }
    public string? TourSlug { get; set; }
}

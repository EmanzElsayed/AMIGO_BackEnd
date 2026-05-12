using System;

namespace Amigo.Domain.DTO.Favorite;

public class ToggleFavoriteResponseDTO
{
    public Guid TourId { get; set; }
    public bool IsFavorite { get; set; }
}

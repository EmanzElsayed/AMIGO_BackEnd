using Amigo.SharedKernal.DTOs.Images;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Review
{
    public record AdminAddReviewsDTO
    (
            string UserName,
            string UserNationality,
            decimal Rating,
            string Comment,
            string? Language,
            string? TravelWith,
            List<ImageUrlsForReviewRequestDTO>? ImageUrls,
            List<Guid> ToursId
    );
}

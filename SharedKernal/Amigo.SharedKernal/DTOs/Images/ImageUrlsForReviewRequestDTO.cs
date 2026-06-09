using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Images
{
    public record ImageUrlsForReviewRequestDTO(
        string? ImageUrl,
        string? PublicId
     );
}

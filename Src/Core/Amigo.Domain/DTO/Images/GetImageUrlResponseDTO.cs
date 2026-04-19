using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Images
{
    public record GetImageUrlResponseDTO
     (
        Guid id,
        string ImageUrl
     );
}

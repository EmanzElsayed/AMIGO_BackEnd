using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Images
{
    public record UploadImageResponseDTO
    (
        string ImageUrl,
        string PublicId
        );
}

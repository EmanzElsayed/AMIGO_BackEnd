using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Images
{
    public record UploadMultiImagesRequestDTO
    (
        List<IFormFile>? Images
       );
}

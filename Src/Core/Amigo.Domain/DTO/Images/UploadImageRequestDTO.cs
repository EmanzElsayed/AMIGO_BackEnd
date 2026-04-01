using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Amigo.Domain.DTO.Images
{
    public record UploadImageRequestDTO
    (
       IFormFile? Image
     );
}

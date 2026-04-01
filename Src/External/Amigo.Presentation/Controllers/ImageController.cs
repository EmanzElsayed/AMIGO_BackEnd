using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Images;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1/image")]
    public class ImageController(IImageService _imageService) :BaseController
    {
        [HttpPost("upload")]
        public async Task<IResultBase> UploadImage([FromForm] UploadImageRequestDTO requestDTO)
        {
           return await _imageService.UploadImage(requestDTO);

        }
        [HttpDelete("delete/{publicId}")]
        public async Task<IResultBase> DeleteImage(string publicId)
        {
            return await _imageService.DeleteImage(publicId);
        }
    }
}

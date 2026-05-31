using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Images;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1/image")]
    public class ImageController( IServiceManager _serviceManager) :BaseController
    {
        [HttpPost("upload")]
        public async Task<IResultBase> UploadImage([FromForm] UploadImageRequestDTO requestDTO)
        {
           return await _serviceManager.ImageService.UploadImage(requestDTO);

        }
        [HttpPost("upload-multi")]
        public async Task<IResultBase> UploadMultiImage([FromForm] UploadMultiImagesRequestDTO requestDTO)
        {
            return await _serviceManager.ImageService.UploadMultiImages(requestDTO);
        }





        [HttpDelete("delete")]
        public async Task<IResultBase> DeleteImage([FromBody] DeleteImageRequestDTO requestDTO)
        {
            return await _serviceManager.ImageService.DeleteImage(requestDTO.PublicId);
        }
    }
}

using Amigo.Domain.DTO.Images;
using Amigo.SharedKernal.DTOs.Images;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IImageService
    {
        Task<Result<UploadImageResponseDTO>> UploadImage(UploadImageRequestDTO requestDTO);
        //Task<Result<string>> DeleteImage(string publicId);

        Task<Result<List<UploadImageResponseDTO>>> UploadMultiImages(UploadMultiImagesRequestDTO requestDTO);
    }
}

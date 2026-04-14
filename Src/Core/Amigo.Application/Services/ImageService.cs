using Amigo.Domain.DTO.Images;
using Amigo.SharedKernal.DTOs.Images;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class ImageService(IValidationService _validationService ,
        ImageCloudService _imageCloudService) : IImageService
    {
        public async Task<Result<UploadImageResponseDTO>> UploadImage(UploadImageRequestDTO requestDTO)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
            {
                return validationResult;    
            }
            var uploadResult = _imageCloudService.UploadImage(requestDTO.Image, "Destination");
            if (uploadResult is null)
            {
                return Result.Fail("Image Is Empty");
            }

            var url = uploadResult.SecureUrl.ToString();
            var publicId = uploadResult.PublicId;


            var result = new UploadImageResponseDTO(url,publicId);
            return Result.Ok(result);
        }

        public async Task<Result<List<UploadImageResponseDTO>>> UploadMultiImages(UploadMultiImagesRequestDTO requestDTO)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            if (requestDTO.Images is null || !requestDTO.Images.Any())
            {
                return Result.Fail("No images provided");
            }

            var uploadedImages = new List<UploadImageResponseDTO>();

            foreach (var image in requestDTO.Images)
            {
                var uploadResult = _imageCloudService.UploadImage(image, "Tour");

                if (uploadResult is null)
                    continue; 

                uploadedImages.Add(new UploadImageResponseDTO(
                    uploadResult.SecureUrl.ToString(),
                    uploadResult.PublicId
                ));
            }

            return Result.Ok(uploadedImages);

        }

        public Task UpdateImagesAsync(
            Tour tour,
            List<ImageUrlsRequestDTO>? imagesDto)
        {
            if (imagesDto is null)
                return Task.CompletedTask;

            //  normalize input
            var validDtos = imagesDto
                .Where(x => !string.IsNullOrWhiteSpace(x.PublicId) &&
                            !string.IsNullOrWhiteSpace(x.ImageUrl))
                .ToList();

            var requestPublicIds = validDtos
                .Select(x => x.PublicId!)
                .ToHashSet();

            //  1. Remove images not in request
            var imagesToRemove = tour.Images
                .Where(img => !requestPublicIds.Contains(img.ImagePublicId))
                .ToList();

            foreach (var img in imagesToRemove)
            {
                tour.Images.Remove(img);


                _imageCloudService.DeleteImage(img.ImagePublicId);
            }

            // 2. Add new images
            var existingPublicIds = tour.Images
                .Select(i => i.ImagePublicId)
                .ToHashSet();

            foreach (var dto in validDtos)
            {
                if (existingPublicIds.Contains(dto.PublicId!))
                    continue;

                tour.Images.Add(new TourImage
                {
                    ImageUrl = dto.ImageUrl!,
                    ImagePublicId = dto.PublicId!,
                    TourId = tour.Id
                });
            }

            return Task.CompletedTask;
        }
        //public Task<Result<string>> DeleteImage(string publicId)
        //{
        //    if (string.IsNullOrEmpty(publicId))
        //        return Task.FromResult(Result.Fail<string>("PublicId is required"));

        //    var result = _imageCloudService.DeleteImage(publicId);

        //    return result == "ok"
        //        ? Task.FromResult(Result.Ok("Image deleted successfully"))
        //        : result == "not found"
        //            ? Task.FromResult(Result.Fail<string>("Image not found"))
        //            : Task.FromResult(Result.Fail<string>($"Deletion failed: {result}"));
        //}
    }
}

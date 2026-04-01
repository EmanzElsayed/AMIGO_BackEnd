using Amigo.Application.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Amigo.Application.Services;

public class ImageCloudService
{
    private readonly Cloudinary _cloudinary;

    public ImageCloudService(IOptions<CloudinarySettings> config)
    {
        var settings = config.Value;
        _cloudinary = new Cloudinary(new Account(
            settings.CloudName,
            settings.ApiKey,
            settings.ApiSecret
        ));
    }

    public ImageUploadResult UploadImage(IFormFile file , string folder)
    {
        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream(); 
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder 
            };

            var result = _cloudinary.Upload(uploadParams);
            return result;  
            //return result.SecureUrl.ToString(); 
        }
        return null;
    }
    public string DeleteImage(string publicId)
    {
        var deletionParams = new DeletionParams(publicId) { ResourceType = ResourceType.Image };
        var result = _cloudinary.Destroy(deletionParams);
        return result.Result;
    }
}
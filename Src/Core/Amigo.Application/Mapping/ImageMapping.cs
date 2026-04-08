using Amigo.Domain.DTO.Images;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public class ImageMapping : IImageMapping
    {
        public IEnumerable<TourImage> ImagesToEntity(List<ImageUrlsRequestDTO> requestDTO, Tour tour)
        {
            return requestDTO
             .Where(x => x != null && !string.IsNullOrWhiteSpace(x.ImageUrl))
             .Select(image => new TourImage
             {
                 Id = Guid.NewGuid(),
                 Tour = tour,
                 TourId = tour.Id,
                 ImageUrl = image.ImageUrl!.Trim(),
                 ImagePublicId = image.PublicId
             })
             .ToList();
        }
    }
}

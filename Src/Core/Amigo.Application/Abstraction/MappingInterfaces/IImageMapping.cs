using Amigo.Domain.DTO.Images;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.MappingInterfaces
{
    public interface IImageMapping
    {
        IEnumerable<TourImage> ImagesToEntity(List<ImageUrlsRequestDTO>  requestDTO , Tour tour);

    }
}

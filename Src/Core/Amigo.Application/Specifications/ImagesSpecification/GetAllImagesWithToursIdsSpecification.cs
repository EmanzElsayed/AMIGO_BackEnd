using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.ImagesSpecification
{
    public class GetAllImagesWithToursIdsSpecification : BaseSpecification<TourImage, Guid>
    {
        public GetAllImagesWithToursIdsSpecification(List<Guid> tourIds) 
            : base(img => tourIds.Contains(img.TourId) && !img.IsDeleted)
        {
        }
    }
}

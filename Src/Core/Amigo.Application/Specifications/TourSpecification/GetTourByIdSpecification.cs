using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification
{
    public class GetTourByIdSpecification : BaseSpecification<Tour, Guid>
    {
        public GetTourByIdSpecification(Guid tourId)
            : base(t => t.Id == tourId && t.IsDeleted == false)
        {
            AddInclude(t => t.Translations);
            AddInclude(t => t.Images);

            AddInclude(t => t
               .Include(t => t.Destination)
               .ThenInclude(t => t.Translations)
               );

            
        }
    }
}

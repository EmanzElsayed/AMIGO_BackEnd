using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification
{
    public class GetTourByIdWithPriceIncludingOnlySpecification : BaseSpecification<Tour, Guid>
    {
        public GetTourByIdWithPriceIncludingOnlySpecification(Guid tourId)
            : base(t => t.Id == tourId && t.IsDeleted == false 
            )
        {
            //AddInclude(t => t
            //      .Include(t => t.Prices)
            //      .ThenInclude(t => t.Translations)
            //      );
        }
    }
}

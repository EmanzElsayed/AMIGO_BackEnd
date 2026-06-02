using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification.User
{
    public class PriceWithoutUserTypeForTourSpecification : BaseSpecification<Price, Guid>
    {
        public PriceWithoutUserTypeForTourSpecification(Guid tourId)
            : base(BuildCriteria(tourId))
        {
            AddInclude(p => p.Translations);
        }

        private static Expression<Func<Price, bool>> BuildCriteria(Guid tourId)
        {
            return p => p.TourId == tourId && !p.IsDeleted ;
        }
    
    }
}

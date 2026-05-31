using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification
{
    public class GetInclustionWithTourIdSpecification : BaseSpecification<TourInclusion, Guid>
    {
        public GetInclustionWithTourIdSpecification(Guid tourId) 
            : base(i => !i.IsDeleted && i.TourId == tourId)
        {
            AddInclude(i => i.Translations);
        }
    }
}

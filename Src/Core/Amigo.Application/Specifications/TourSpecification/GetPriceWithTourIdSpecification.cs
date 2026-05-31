using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification
{
    public class GetPriceWithTourIdSpecification : BaseSpecification<Price, Guid>
    {
        public GetPriceWithTourIdSpecification(Guid tourId) 
            : base(p => !p.IsDeleted && p.TourId == tourId)
        {
            AddInclude(p => p.Translations);
        }
    }
}

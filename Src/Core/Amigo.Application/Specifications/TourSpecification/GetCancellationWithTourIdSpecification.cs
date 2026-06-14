using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification
{
    public class GetCancellationWithTourIdSpecification : BaseSpecification<Cancellation, Guid>
    {
        public GetCancellationWithTourIdSpecification(Guid tourId) 
            : base(c => !c.IsDeleted && c.TourId == tourId)
        {
        }
    }
}

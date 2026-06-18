using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CancellationSpecification
{
    public class GetAllCancellationWithToursIdsSpecification : BaseSpecification<Cancellation, Guid>
    {
        public GetAllCancellationWithToursIdsSpecification(HashSet<Guid> toursIds) 
            : base(c => !c.IsDeleted && toursIds.Contains(c.TourId))
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.BlackoutDateSpecification
{
    public class GetBlackoutDatesWithTourIdsSpecification : BaseSpecification<BlackoutDate, Guid>
    {
        public GetBlackoutDatesWithTourIdsSpecification(HashSet<Guid> tourIds) 
            : base(b => !b.IsDeleted && tourIds.Contains(b.TourId))
        {
        }
    }
}

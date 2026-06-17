using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.BlackoutWeekDaysSpecification
{
    public class GetBlackoutWeekDaysWithTourIdsSpecification : BaseSpecification<BlackoutWeekDay, Guid>
    {
        public GetBlackoutWeekDaysWithTourIdsSpecification(HashSet<Guid>tourIds)
            : base(w => !w.IsDeleted && tourIds.Contains(w.TourId))
        {
        }
    }
}

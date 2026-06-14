using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.BlackoutWeekDaysSpecification
{
    public class GetBlackoutWeekDayesSpecification : BaseSpecification<BlackoutWeekDay, Guid>
    {
        public GetBlackoutWeekDayesSpecification(Guid tourId) 
            : base(w => !w.IsDeleted && w.TourId == tourId)
        {
        }
    }
}

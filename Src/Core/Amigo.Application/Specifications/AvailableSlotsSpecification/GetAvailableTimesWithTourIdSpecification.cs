using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.AvailableSlotsSpecification
{
    public class GetAvailableTimesWithTourIdSpecification : BaseSpecification<AvailableSlots, Guid>
    {
        public GetAvailableTimesWithTourIdSpecification(Guid tourId)
            : base(s => !s.IsDeleted && s.TourId == tourId)
        {
        }
    }
}

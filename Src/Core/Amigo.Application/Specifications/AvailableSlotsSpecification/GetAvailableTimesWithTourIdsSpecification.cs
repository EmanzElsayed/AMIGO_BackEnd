using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.AvailableSlotsSpecification
{
    public class GetAvailableTimesWithTourIdsSpecification : BaseSpecification<AvailableSlots, Guid>
    {
        public GetAvailableTimesWithTourIdsSpecification(HashSet<Guid>tourIds) 
            : base(a => !a.IsDeleted && tourIds.Contains(a.TourId) && a.AvailableTimeStatus == AvailableDateTimeStatus.Available)
        {
        }
    }
}

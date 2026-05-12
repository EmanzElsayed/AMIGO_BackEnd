using Amigo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.AvailableSlotsSpecification
{
    public class GetSlotsByDateSpecification : BaseSpecification<AvailableSlots, Guid>
    {
        public GetSlotsByDateSpecification(DateTime startDate, DateTime endDate)
            : base(s => !s.IsDeleted && s.TourSchedule.StartDate >= DateOnly.FromDateTime(startDate) && s.TourSchedule.StartDate < DateOnly.FromDateTime(endDate))
        {
            AddInclude(s => s.TourSchedule);
        }
    }
}

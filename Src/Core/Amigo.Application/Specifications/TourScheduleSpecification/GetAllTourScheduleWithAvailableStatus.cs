using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.TourScheduleSpecification
{
    public class GetAllTourScheduleWithAvailableStatus : BaseSpecification<TourSchedule, Guid>
    {
        public GetAllTourScheduleWithAvailableStatus() 
            : base(t => !t.IsDeleted &&  t.AvailableDateStatus == AvailableDateTimeStatus.Available)
        {
            AddInclude(t => t.AvailableSlots);
        }
    }
}

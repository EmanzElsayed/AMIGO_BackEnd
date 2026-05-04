using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.BookingSpecification
{
    public class GetBookingsForReminderSpecification : BaseSpecification<Booking, Guid>
    {
        public GetBookingsForReminderSpecification(DateTime from , DateTime to)
            : base(b => !b.IsDeleted 
            && b.Status == BookingStatus.Confirmed 
            &&  b.OrderItem.TourDate.ToDateTime(b.OrderItem.StartTime) >= from 
            && b.OrderItem.TourDate.ToDateTime(b.OrderItem.StartTime) <= to)
        {
        }
    }
}

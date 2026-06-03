using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.BookingSpecification
{
    public class GetBookingByIdIncludingOrderItemSpecification : BaseSpecification<Booking, Guid>
    {
        public GetBookingByIdIncludingOrderItemSpecification(Guid bookingId)
            : base(b => b.Id == bookingId && b.IsDeleted == false)
        {
            AddInclude(b => b.OrderItem);
        }
    
    }
}

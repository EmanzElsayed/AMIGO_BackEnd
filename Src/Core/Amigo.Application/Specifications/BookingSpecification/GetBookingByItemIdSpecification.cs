using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.BookingSpecification
{
    public class GetBookingByItemIdSpecification : BaseSpecification<Booking, Guid>
    {
        public GetBookingByItemIdSpecification(Guid itemId) 
            : base(b => b.OrderItemId == itemId)
        {
        }
    }
}

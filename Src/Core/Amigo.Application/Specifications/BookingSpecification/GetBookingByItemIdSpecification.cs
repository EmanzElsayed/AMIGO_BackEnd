using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.BookingSpecification
{
    public class GetBookingByItemIdSpecification : UserBaseSpecification<Booking, Guid>
    {
        public GetBookingByItemIdSpecification(Guid itemId) 
            : base(b => b.OrderItemId == itemId)
        {
        }
    }
}

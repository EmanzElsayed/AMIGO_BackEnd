using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.OrderSpecification
{
    public class GetOrderItemByBookingIdSpecification : BaseSpecification<OrderItem, Guid>
    {
        public GetOrderItemByBookingIdSpecification(Guid bookingId) 
            : base(o => !o.IsDeleted && o.Booking.Id == bookingId)
        {
            AddInclude(o => o.OrderedPrice);
        }
    }
}

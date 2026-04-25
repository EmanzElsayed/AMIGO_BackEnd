using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.BookingSpecification
{
    public class GetBookingsByOrderItemIdsSpecification
        : BaseSpecification<Booking, Guid>
    {
        public GetBookingsByOrderItemIdsSpecification(List<Guid> orderItemIds)
            : base(b => orderItemIds.Contains(b.OrderItemId) && !b.IsDeleted)
        {
        }
    }
}

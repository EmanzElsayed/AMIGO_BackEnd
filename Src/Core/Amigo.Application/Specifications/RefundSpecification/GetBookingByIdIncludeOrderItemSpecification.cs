using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.RefundSpecification
{
    internal class GetBookingByIdIncludeOrderItemSpecification : BaseSpecification<Booking, Guid>
    {
        public GetBookingByIdIncludeOrderItemSpecification(Guid bookingId)
            : base(b => b.Id == bookingId && b.IsDeleted == false)
        {
            AddInclude(b => b.OrderItem);
        }
    }
   
}

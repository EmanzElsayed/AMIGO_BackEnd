using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.RefundSpecification
{
    public class GetRefundByBookingIdSpecification : BaseSpecification<Refund, Guid>
    {
        public GetRefundByBookingIdSpecification(Guid bookingId) 
            : base(r => r.BookingId == bookingId && r.Status != RefundStatus.Failed)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.RefundSpecification
{
    public class GetCancellationRequestByBookingIdSpecification : BaseSpecification<CancellationRequest, Guid>
    {
        public GetCancellationRequestByBookingIdSpecification(Guid bookingId) 
            : base(c => !c.IsDeleted && c.BookingId == bookingId)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.BookingSpecification
{
    public class GetBookingNotSendVoucherSpecifciation : BaseSpecification<Booking, Guid>
    {
        public GetBookingNotSendVoucherSpecifciation() 
            : base(b => !b.IsDeleted && !b.IsVoucherCreated && b.Status == BookingStatus.Confirmed)
        {
            AddInclude(b => b.Travelers);
            AddInclude(b => b.OrderItem);

        }
    }
}

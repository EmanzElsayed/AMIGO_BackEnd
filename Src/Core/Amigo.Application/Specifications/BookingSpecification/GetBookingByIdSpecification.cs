using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.BookingSpecification
{
    public class GetBookingByIdSpecification : UserBaseSpecification<Booking, Guid>
    {
        public GetBookingByIdSpecification(Guid bookingId) 
            : base(b => b.Id  == bookingId && b.IsDeleted == false)
        {
            AddInclude(b => b.Travelers);

        }
    }
}

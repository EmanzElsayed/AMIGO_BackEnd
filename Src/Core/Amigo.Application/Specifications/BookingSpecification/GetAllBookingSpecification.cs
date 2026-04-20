using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.BookingSpecification
{
    public class GetAllBookingSpecification : BaseSpecification<Booking, Guid>
    {
        public GetAllBookingSpecification() 
            : base(b => !b.IsDeleted && b.Status == BookingStatus.Confirmed)
        {
            AddInclude(b => b.User);
        }
    }
}

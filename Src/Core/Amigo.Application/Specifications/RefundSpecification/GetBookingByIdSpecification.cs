using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.RefundSpecification
{
    public class GetBookingByIdSpecification : BaseSpecification<Booking, Guid>
    {
        public GetBookingByIdSpecification(Guid bookingId)
            : base(b => b.Id == bookingId && b.IsDeleted == false)
        {
           
        }
    }
   
}

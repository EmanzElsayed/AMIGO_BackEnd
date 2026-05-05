using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.BookingSpecification
{
    public class GetBookingWithVoucherTokenSpecification : BaseSpecification<Booking, Guid>
    {
        public GetBookingWithVoucherTokenSpecification(string token)
            : base(v => !v.IsDeleted && !string.IsNullOrWhiteSpace(v.VoucherToken) && v.VoucherToken == token)
        {
        }
    }
}

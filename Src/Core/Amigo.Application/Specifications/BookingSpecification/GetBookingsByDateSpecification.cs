using Amigo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.BookingSpecification
{
    public class GetBookingsByDateSpecification : BaseSpecification<Booking, Guid>
    {
        public GetBookingsByDateSpecification(DateTime startDate, DateTime endDate)
            : base(b => !b.IsDeleted && b.ConfirmedAt >= startDate && b.ConfirmedAt < endDate)
        {
            AddInclude(b => b.OrderItem);
        }
    }
}

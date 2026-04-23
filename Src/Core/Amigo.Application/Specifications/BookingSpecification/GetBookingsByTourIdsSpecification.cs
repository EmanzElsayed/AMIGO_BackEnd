using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amigo.Application.Specifications.BookingSpecification
{
    public class GetBookingsByTourIdsSpecification : UserBaseSpecification<Booking, Guid>
    {
        public GetBookingsByTourIdsSpecification(List<Guid> tourIds)
            : base(b =>
                !b.IsDeleted &&
                b.Status == BookingStatus.Confirmed && b.OrderItem.TourId != null &&
                tourIds.Contains(b.OrderItem.TourId.Value))
        {
            AddInclude(b => b.OrderItem);
        }
    }
}

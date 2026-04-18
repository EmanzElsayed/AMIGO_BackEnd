using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.DestinationSpecification.Admin
{
    //public class ActiveBookingsForDestinationSpecification : BaseSpecification<Booking, Guid>
    //{
    //    public ActiveBookingsForDestinationSpecification(Guid destinationId)
    //        : base(b =>
    //            b.Status == BookingStatus.Confirmed &&
    //            b.AvailableSlots.TourSchedule.Tour.DestinationId == destinationId
    //        )
    //    {
    //        AddInclude(b => b.AvailableSlots);
    //        AddInclude(b => b.AvailableSlots.TourSchedule);
    //        AddInclude(b => b.AvailableSlots.TourSchedule.Tour);
    //    }
    //}
}

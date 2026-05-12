using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification
{
    public class GetConfirmedReservationsSpecification : BaseSpecification<SlotReservation, Guid>
    {
        public GetConfirmedReservationsSpecification()
            : base(r => !r.IsDeleted && r.Status == ReservationStatus.Confirmed)
        {
        }
    }
}

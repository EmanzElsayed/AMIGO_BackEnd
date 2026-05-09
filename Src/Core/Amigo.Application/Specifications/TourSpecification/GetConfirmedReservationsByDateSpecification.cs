using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification
{
    public class GetConfirmedReservationsByDateSpecification : BaseSpecification<SlotReservation, Guid>
    {
        public GetConfirmedReservationsByDateSpecification(DateTime startDate, DateTime endDate)
            : base(r => !r.IsDeleted && r.Status == ReservationStatus.Confirmed && r.CreatedAt >= startDate && r.CreatedAt < endDate)
        {
        }
    }
}

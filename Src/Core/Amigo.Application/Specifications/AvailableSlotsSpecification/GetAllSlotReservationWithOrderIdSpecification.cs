using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.AvailableSlotsSpecification
{
    internal class GetAllSlotReservationWithOrderIdSpecification : BaseSpecification<SlotReservation, Guid>
    {
        public GetAllSlotReservationWithOrderIdSpecification(Guid orderId)
            : base(r => r.OrderId == orderId && r.Status == ReservationStatus.Pending && r.ExpiresAt > DateTime.UtcNow)
        {
        }
    }
}

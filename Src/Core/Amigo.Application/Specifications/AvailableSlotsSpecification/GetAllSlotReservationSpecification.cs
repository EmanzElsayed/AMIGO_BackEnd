using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.AvailableSlotsSpecification
{
    public class GetAllSlotReservationSpecification : UserBaseSpecification<SlotReservation, Guid>
    {
        public GetAllSlotReservationSpecification(Guid slotId) 
            : base(r => r.SlotId == slotId && r.Status == ReservationStatus.Pending && r.ExpiresAt > DateTime.UtcNow)
        {
        }
    }
}

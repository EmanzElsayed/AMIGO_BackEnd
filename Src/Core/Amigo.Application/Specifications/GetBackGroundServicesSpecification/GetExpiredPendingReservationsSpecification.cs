using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.GetBackGroundServicesSpecification
{
    public class GetExpiredPendingReservationsSpecification
    : BaseSpecification<SlotReservation, Guid>
    {
        public GetExpiredPendingReservationsSpecification(DateTime now)
            : base(r =>
                r.Status == ReservationStatus.Pending &&
                r.ExpiresAt < now && !r.IsDeleted)
        {
            AddInclude(r => r.Slot);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.GetBackGroundServicesSpecification
{
    public class GetOldExpiredReservationsSpecification
    : BaseSpecification<SlotReservation, Guid>
    {
        public GetOldExpiredReservationsSpecification(DateTime oldDate)
            : base(r =>
                r.Status == ReservationStatus.Expired &&
                r.CreatedAt < oldDate)
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.AvailableSlotsSpecification
{
    internal class GetReservationsBySlotIdsSpecification : BaseSpecification<SlotReservation, Guid>
    {
        public GetReservationsBySlotIdsSpecification(List<Guid> ids)
            : base(t => ids.Contains(t.Id) && !t.IsDeleted)
        {
    }   }
}

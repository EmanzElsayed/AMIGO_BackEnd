using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.AvailableSlotsSpecification
{
    public class GetSlotByIdIncludeReservationSpecification : BaseSpecification<AvailableSlots, Guid>
    {
        public GetSlotByIdIncludeReservationSpecification(Guid slotId) 
            : base(s => !s.IsDeleted && s.Id == slotId)
        {
            AddInclude(s => s.SlotReservations);
        }
    }
}

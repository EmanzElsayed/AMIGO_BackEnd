using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.AvailableSlotsSpecification
{
    public class GetSlotsByIdsSpecification
        : BaseSpecification<AvailableSlots, Guid>
    {
        public GetSlotsByIdsSpecification(List<Guid> ids)
            : base(s => ids.Contains(s.Id) && !s.IsDeleted && s.AvailableTimeStatus == AvailableDateTimeStatus.Available)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.AvailableSlotsSpecification
{
    public class GetTourSlotsByTourIdSpecification : BaseSpecification<AvailableSlots, Guid>
    {
        public GetTourSlotsByTourIdSpecification(Guid tourId)
            : base(s => !s.IsDeleted && s.TourId == tourId && s.AvailableTimeStatus == AvailableDateTimeStatus.Available)
        {
        }
    }
}

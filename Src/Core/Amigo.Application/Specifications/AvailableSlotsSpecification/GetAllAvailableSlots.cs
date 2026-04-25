using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.AvailableSlotsSpecification
{
    public class GetAllAvailableSlots : BaseSpecification<AvailableSlots, Guid>
    {
        public GetAllAvailableSlots()
            : base(s => !s.IsDeleted && s.AvailableTimeStatus == AvailableDateTimeStatus.Available)
        {
        }
    }
}

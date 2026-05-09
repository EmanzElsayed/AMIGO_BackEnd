using Amigo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.AvailableSlotsSpecification
{
    public class GetAllSlotsSpecification : BaseSpecification<AvailableSlots, Guid>
    {
        public GetAllSlotsSpecification() : base(s => !s.IsDeleted)
        {
        }
    }
}

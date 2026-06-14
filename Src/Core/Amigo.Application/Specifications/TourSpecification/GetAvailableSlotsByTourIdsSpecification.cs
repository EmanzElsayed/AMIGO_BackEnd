using Amigo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification
{
    public class GetAvailableSlotsByTourIdsSpecification
      : BaseSpecification<AvailableSlots, Guid>
    {
        public GetAvailableSlotsByTourIdsSpecification(List<Guid> tourIds)
            : base(s => tourIds.Contains(
                s.TourId))
        {
          
        }
    }
}

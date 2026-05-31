using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification
{
    public class GetPricesByTourIdsSpecification : BaseSpecification<Price, Guid>
    {
        public GetPricesByTourIdsSpecification(List<Guid> tourIds)
            : base(p => tourIds.Contains(p.TourId))
        {
            AddInclude(p => p.Translations);
        }
    }
}

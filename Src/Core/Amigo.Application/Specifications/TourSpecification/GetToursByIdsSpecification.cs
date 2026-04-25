using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification;

public class GetToursByIdsSpecification : BaseSpecification<Tour, Guid>
{
    public GetToursByIdsSpecification(List<Guid> ids)
        : base(t => ids.Contains(t.Id) && !t.IsDeleted)
    {
        AddInclude(t => t.Translations);
        AddInclude(t => t.Images);



        AddInclude(t => t
               .Include(t => t.AvailableTimes)
               .ThenInclude(t => t.AvailableSlots)
               );



        AddInclude(t => t
              .Include(t => t.Cancellation)
              .ThenInclude(t => t.Translations)
              );

        AddInclude(t => t
            .Include(t => t.TourInclusions)
            .ThenInclude(t => t.Translations)
            );

        AddInclude(t => t
           .Include(t => t.Destination)
           .ThenInclude(t => t.Translations)
           );

        AddInclude(t => t
                .Include(t => t.Prices)
                .ThenInclude(t => t.Translations)
                );
    }
}
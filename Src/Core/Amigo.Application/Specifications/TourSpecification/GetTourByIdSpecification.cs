using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification
{
    public class GetTourByIdSpecification : BaseSpecification<Tour, Guid>
    {
        public GetTourByIdSpecification(Guid tourId)
            : base(t => t.Id == tourId && t.IsDeleted == false)
        {
            AddInclude(t => t.Translations);



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
}

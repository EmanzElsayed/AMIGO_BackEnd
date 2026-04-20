using Amigo.Application.Helpers;
using Amigo.Application.Validation.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification
{
    public class GetAllToursForAdminSpecification : BaseSpecification<Tour, Guid>
    {
        public GetAllToursForAdminSpecification(GetAllAdminTourQuery requestQuery)
            : base(TourCatalogCriteria.BuildAdminTourCatalog(requestQuery.DestinationId,requestQuery.Name,requestQuery.Language))
            
        {
            ApplyPagination(requestQuery.PageSize, requestQuery.PageNumber);

            AddOrderBYDescending(t => t.CreatedDate);

            AddInclude(t => t.Translations

               );

            AddInclude(t => t
               .Include(t => t.Destination)
               .ThenInclude(t => t.Translations
               )
               );

            AddInclude(t => t
                    .Include(t => t.Prices)
                    .ThenInclude(t => t.Translations)
                    );

            AddInclude(t => t
                  .Include(t => t.AvailableTimes)
                  .ThenInclude(t => t.AvailableSlots)
                  );

            AddInclude(t => t.Images);

        }
    }
}

using Amigo.Application.Validation.Common.Specifications;
using System;
using Amigo.SharedKernal.QueryParams;

namespace Amigo.Application.Specifications.TourSpecification
{
    public class CountAllToursForAdminSpecification : BaseSpecification<Tour, Guid>
    {
        public CountAllToursForAdminSpecification(GetAllAdminTourQuery requestQuery)
            : base(TourCatalogCriteria.BuildAdminTourCatalog(requestQuery.DestinationId, requestQuery.Name, requestQuery.Language, requestQuery.FilterActiveOnly))
        {
        }
    }
}

using Amigo.Application.Validation.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification.User
{
    internal class CountGetAllTourForAdminSpecification: BaseSpecification<Tour, Guid>
    {
        public CountGetAllTourForAdminSpecification(GetAllAdminTourQuery requestQuery)
            : base(TourCatalogCriteria.BuildAdminTourCatalog(requestQuery.DestinationId, requestQuery.Name, requestQuery.Language))

        { }
        
    }
}

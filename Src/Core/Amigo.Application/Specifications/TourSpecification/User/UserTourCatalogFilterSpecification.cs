using Amigo.Application.Specifications;
using Amigo.Application.Validation.Common.Specifications;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.QueryParams;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class UserTourCatalogFilterSpecification : BaseSpecification<Tour, Guid>
{
    public UserTourCatalogFilterSpecification(
        GetUserToursQuery query,
        Guid destinationId,
        Language? translationLanguage,
        Language? effectiveGuideLanguage,
        CurrencyCode? currencyFilter,
        CountryCode? destinationCountryFilter,
        UserType? userTypeFilter,
        DateOnly? availabilityDate)
        : base(TourCatalogCriteria.Build(
            destinationId,
            query,
            translationLanguage,
            effectiveGuideLanguage,
            currencyFilter,
            destinationCountryFilter,
            userTypeFilter,
            availabilityDate))
    {
    }
}

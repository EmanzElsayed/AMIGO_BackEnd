using Amigo.Application.Specifications;
using Amigo.Application.Validation.Common.Specifications;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.QueryParams;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class UserTourCatalogSpecification : BaseSpecification<Tour, Guid>
{
    public UserTourCatalogSpecification(
        GetUserToursQuery query,
        Guid destinationId,
        Language? translationLanguage,
        Language? effectiveGuideLanguage,
        Currency? currencyFilter,
        CountryCode? destinationCountryFilter,
        UserType? userTypeFilter,
        DateOnly? availabilityDate,
        bool applyPaging)
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
        AddInclude(t => t.Translations);
        AddInclude(t => t.Images);
        AddInclude(t => t.Prices);
        AddInclude(t => t.Reviews);
        AddInclude(t => t.Cancellation!);
        AddInclude(t => t.TourInclusions);

        if (applyPaging)
            ApplyPagination(query.PageSize, query.PageNumber);
    }
}

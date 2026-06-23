using Amigo.Domain.Entities;
using System.Linq.Expressions;

namespace Amigo.Application.Validation.Common.Specifications;

/// <summary>
/// Reusable criteria for ranking / listing catalog destinations (active, not deleted, bookable catalog).
/// </summary>
public static class TopDestinationCommonSpecification
{
    public static Expression<Func<Destination, bool>> BuildRankingEligibleCriteria(CountryCode? countryCode)
    {
        return d =>
            d.IsActive
            && !d.IsDeleted
            &&(countryCode == null || d.CountryInfo.CountryCode == countryCode);
    }
}

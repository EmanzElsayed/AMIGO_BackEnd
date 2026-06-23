using Amigo.Application.Specifications;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class TrendingToursSpecification : BaseSpecification<Tour, Guid>
{
    public TrendingToursSpecification(DateOnly todayUtc,CountryCode? countryCode)
        : base(BuildCriteria(todayUtc , countryCode))
    {
        AddInclude(t => t.Translations);
        AddInclude(t => t.Images);
        //AddInclude(t => t.Prices);
        //AddInclude(t => t.Reviews);
        AddInclude(t => t.Destination);
        AddInclude(t => t.Destination.Translations);
        //AddInclude(t => t.AvailableTimes);
        AddOrderBYDescending(t => t.Reviews.Count);
    }

    private static Expression<Func<Tour, bool>> BuildCriteria(DateOnly todayUtc , CountryCode? countryCode)
    {
        return t =>
            !t.IsDeleted
            && !t.Destination.IsDeleted
            && t.Destination.IsActive
            && (countryCode == null || t.Destination.CountryInfo.CountryCode == countryCode)
            ;
    }
}

using Amigo.Application.Specifications;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class TrendingToursSpecification : BaseSpecification<Tour, Guid>
{
    public TrendingToursSpecification(DateOnly todayUtc)
        : base(BuildCriteria(todayUtc))
    {
        AddInclude(t => t.Translations);
        AddInclude(t => t.Images);
        AddInclude(t => t.Prices);
        AddInclude(t => t.Reviews);
        AddInclude(t => t.Destination);
        AddInclude(t => t.Destination.Translations);
        AddInclude(t => t.AvailableTimes);
        AddOrderBYDescending(t => t.Reviews.Count);
    }

    private static Expression<Func<Tour, bool>> BuildCriteria(DateOnly todayUtc)
    {
        return t =>
            !t.IsDeleted
            && !t.Destination.IsDeleted
            && t.Destination.IsActive
            && t.AvailableTimes.Any(ts =>
                !ts.IsDeleted
                && ts.AvailableDateStatus == AvailableDateTimeStatus.Available
                && ts.StartDate >= todayUtc
                && ts.AvailableSlots.Any(s =>
                    !s.IsDeleted
                    && s.AvailableTimeStatus == AvailableDateTimeStatus.Available));
    }
}

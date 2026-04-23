using Amigo.Application.Specifications;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.User;


public class ToursWithFutureSchedulesForDestinationSpecification : UserBaseSpecification<Tour, Guid>
{
    public ToursWithFutureSchedulesForDestinationSpecification(Guid destinationId, DateOnly todayUtc)
        : base(BuildCriteria(destinationId, todayUtc))
    {
    }

    private static Expression<Func<Tour, bool>> BuildCriteria(Guid destinationId, DateOnly todayUtc)
    {
        return t =>
            t.DestinationId == destinationId
            && !t.IsDeleted
            && t.AvailableTimes.Any(ts =>
                !ts.IsDeleted
                && (ts.EndDate == null || ts.EndDate >= todayUtc)
                && ts.AvailableSlots.Any(s =>
                    !s.IsDeleted
                    && s.AvailableTimeStatus == AvailableDateTimeStatus.Available));
    }
}

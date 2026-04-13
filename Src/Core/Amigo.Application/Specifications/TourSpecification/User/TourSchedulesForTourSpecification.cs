using Amigo.Application.Specifications;
using Amigo.Domain.Entities;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class TourSchedulesForTourSpecification : BaseSpecification<TourSchedule, Guid>
{
    public TourSchedulesForTourSpecification(Guid tourId)
        : base(BuildCriteria(tourId))
    {
        AddInclude(s => s.AvailableSlots);
    }

    private static Expression<Func<TourSchedule, bool>> BuildCriteria(Guid tourId)
    {
        return s => s.TourId == tourId && !s.IsDeleted;
    }
}

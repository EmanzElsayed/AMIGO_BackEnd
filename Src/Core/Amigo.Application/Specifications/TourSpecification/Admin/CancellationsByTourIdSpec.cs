using Amigo.Application.Specifications;
using Amigo.Domain.Entities;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.Admin;

public class CancellationsByTourIdSpec : BaseSpecification<Cancellation, Guid>
{
    public CancellationsByTourIdSpec(Guid tourId)
        : base(BuildCriteria(tourId))
    {
        //AddInclude(c => c.Translations);
    }

    private static Expression<Func<Cancellation, bool>> BuildCriteria(Guid tourId)
    {
        return c => c.TourId == tourId && !c.IsDeleted;
    }
}

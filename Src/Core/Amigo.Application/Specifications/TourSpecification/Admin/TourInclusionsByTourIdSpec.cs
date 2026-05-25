using Amigo.Application.Specifications;
using Amigo.Domain.Entities;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.Admin;

public class TourInclusionsByTourIdSpec : BaseSpecification<TourInclusion, Guid>
{
    public TourInclusionsByTourIdSpec(Guid tourId)
        : base(BuildCriteria(tourId))
    {
        AddInclude(i => i.Translations);
    }

    private static Expression<Func<TourInclusion, bool>> BuildCriteria(Guid tourId)
    {
        return i => i.TourId == tourId && !i.IsDeleted;
    }
}

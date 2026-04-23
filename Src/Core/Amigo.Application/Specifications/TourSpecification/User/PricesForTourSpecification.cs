using Amigo.Application.Specifications;
using Amigo.Domain.Entities;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class PricesForTourSpecification : UserBaseSpecification<Price, Guid>
{
    public PricesForTourSpecification(Guid tourId)
        : base(BuildCriteria(tourId))
    {
        AddInclude(p => p.Translations);
    }

    private static Expression<Func<Price, bool>> BuildCriteria(Guid tourId)
    {
        return p => p.TourId == tourId && !p.IsDeleted;
    }
}

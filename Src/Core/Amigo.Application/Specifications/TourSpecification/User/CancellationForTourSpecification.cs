using Amigo.Application.Specifications;
using Amigo.Domain.Entities.TranslationEntities;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class CancellationForTourSpecification : BaseSpecification<Cancellation, Guid>
{
    public CancellationForTourSpecification(Guid tourId)
        : base(BuildCriteria(tourId))
    {
        AddInclude(c => c.Translations);
    }

    private static Expression<Func<Cancellation, bool>> BuildCriteria(Guid tourId)
    {
        return x => x.TourId == tourId && !x.IsDeleted;
    }
}

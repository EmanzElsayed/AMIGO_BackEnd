using Amigo.Application.Specifications;
using Amigo.Domain.Entities.TranslationEntities;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class CancellationTranslationsForCancellationSpecification : BaseSpecification<CancellationTranslation, Guid>
{
    public CancellationTranslationsForCancellationSpecification(Guid cancellationId)
        : base(BuildCriteria(cancellationId))
    {
    }

    private static Expression<Func<CancellationTranslation, bool>> BuildCriteria(Guid cancellationId)
    {
        return x => x.CancellationId == cancellationId && !x.IsDeleted;
    }
}

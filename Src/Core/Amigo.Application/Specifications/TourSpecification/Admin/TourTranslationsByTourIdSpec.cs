using Amigo.Application.Specifications;
using Amigo.Domain.Entities.TranslationEntities;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.Admin;

public class TourTranslationsByTourIdSpec : BaseSpecification<TourTranslation, Guid>
{
    public TourTranslationsByTourIdSpec(Guid tourId)
        : base(BuildCriteria(tourId))
    {
    }

    private static Expression<Func<TourTranslation, bool>> BuildCriteria(Guid tourId)
    {
        return t => t.TourId == tourId && !t.IsDeleted;
    }
}

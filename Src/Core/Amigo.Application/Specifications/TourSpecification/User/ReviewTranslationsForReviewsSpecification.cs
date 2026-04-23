using Amigo.Application.Specifications;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.Domain.Enum;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class ReviewTranslationsForReviewsSpecification : UserBaseSpecification<ReviewTranslation, Guid>
{
    public ReviewTranslationsForReviewsSpecification(IReadOnlyCollection<Guid> reviewIds, Language language)
        : base(BuildCriteria(reviewIds, language))
    {
    }

    private static Expression<Func<ReviewTranslation, bool>> BuildCriteria(
        IReadOnlyCollection<Guid> reviewIds,
        Language language)
    {
        return rt => reviewIds.Contains(rt.ReviewId) && rt.Language == language && !rt.IsDeleted;
    }
}

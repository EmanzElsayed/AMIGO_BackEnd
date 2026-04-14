using Amigo.Application.Specifications;
using Amigo.Domain.Entities;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class ReviewsForTourSpecification : BaseSpecification<Review, Guid>
{
    public ReviewsForTourSpecification(Guid tourId)
        : base(BuildCriteria(tourId))
    {
        AddInclude(r => r.User);
        AddInclude(r => r.Images);
    }

    private static Expression<Func<Review, bool>> BuildCriteria(Guid tourId)
    {
        return r => r.TourId == tourId && !r.IsDeleted;
    }
}

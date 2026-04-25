using Amigo.Application.Specifications;
using Amigo.Domain.Entities;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class TourExistsByIdSpecification : BaseSpecification<Tour, Guid>
{
    public TourExistsByIdSpecification(Guid tourId)
        : base(BuildCriteria(tourId))
    {
    }

    private static Expression<Func<Tour, bool>> BuildCriteria(Guid tourId)
    {
        return t => t.Id == tourId && !t.IsDeleted;
    }
}

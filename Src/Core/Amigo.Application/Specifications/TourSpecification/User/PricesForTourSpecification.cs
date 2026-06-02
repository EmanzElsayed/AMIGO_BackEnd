using Amigo.Application.Specifications;
using Amigo.Domain.Entities;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class PricesForTourSpecification : BaseSpecification<Price, Guid>
{
    public PricesForTourSpecification(Guid tourId, UserType allowedUserType)
        : base(BuildCriteria(tourId ,  allowedUserType))
    {
        AddInclude(p => p.Translations);
    }

    private static Expression<Func<Price, bool>> BuildCriteria(Guid tourId , UserType allowedUserType)
    {
        return p => p.TourId == tourId && !p.IsDeleted && (p.UserType & allowedUserType) == allowedUserType ;
    }
}

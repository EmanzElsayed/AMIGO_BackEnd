using Amigo.Application.Specifications;
using Amigo.Domain.Entities;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class UserTourReviewExistsSpecification : UserBaseSpecification<Review, Guid>
{
    public UserTourReviewExistsSpecification(string userId, Guid tourId)
        : base(BuildCriteria(userId, tourId))
    {
    }

    private static Expression<Func<Review, bool>> BuildCriteria(string userId, Guid tourId)
    {
        return r => !r.IsDeleted && r.TourId == tourId && r.UserId == userId;
    }
}

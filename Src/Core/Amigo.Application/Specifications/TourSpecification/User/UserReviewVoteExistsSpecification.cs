using Amigo.Domain.Entities;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class UserReviewVoteExistsSpecification : BaseSpecification<ReviewVote, Guid>
{
    public UserReviewVoteExistsSpecification(Guid reviewId, string? userId, string? ipAddress)
        : base(x => x.ReviewId == reviewId && 
                  ((userId != null && x.UserId == userId) || 
                   (userId == null && ipAddress != null && x.IpAddress == ipAddress)))
    {
    }
}

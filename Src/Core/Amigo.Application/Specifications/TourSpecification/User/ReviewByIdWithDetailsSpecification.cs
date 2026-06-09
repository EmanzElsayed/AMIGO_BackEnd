using Amigo.Application.Specifications;
using Amigo.Domain.Entities;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class ReviewByIdWithDetailsSpecification : BaseSpecification<Review, Guid>
{
    public ReviewByIdWithDetailsSpecification(Guid id)
        : base(r => r.Id == id && !r.IsDeleted)
    {
        
        AddInclude(r => r.Images);
    }
}

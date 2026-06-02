using Amigo.Application.Specifications;
using Amigo.Domain.Entities;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class TourCatalogForSlugResolutionSpecification : BaseSpecification<Tour, Guid>
{
    public TourCatalogForSlugResolutionSpecification(Guid destinationId)
        : base(t => t.DestinationId == destinationId && !t.IsDeleted)
    {
        AddInclude(t => t.Translations);
        AddInclude(t => t.Images);
      
    }
}

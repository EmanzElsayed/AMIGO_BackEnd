using Amigo.Application.Specifications;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class TourIncludedLinesForDestinationSpecification : UserBaseSpecification<TourInclusion, Guid>
{
    public TourIncludedLinesForDestinationSpecification(Guid destinationId)
        : base(ti =>
            !ti.IsDeleted
            && !ti.Tour.IsDeleted
            && ti.Tour.DestinationId == destinationId
            && ti.IsIncluded
            
            )
            
    {
        AddInclude(ti => ti.Translations);
    }
}

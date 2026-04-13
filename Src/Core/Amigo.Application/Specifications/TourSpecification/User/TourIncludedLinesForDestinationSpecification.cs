using Amigo.Application.Specifications;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;

namespace Amigo.Application.Specifications.TourSpecification.User;

public class TourIncludedLinesForDestinationSpecification : BaseSpecification<TourIncluded, Guid>
{
    public TourIncludedLinesForDestinationSpecification(Guid destinationId, Language language)
        : base(ti =>
            !ti.IsDeleted
            && !ti.Tour.IsDeleted
            && ti.Tour.DestinationId == destinationId
            && ti.Language == language)
    {
    }
}

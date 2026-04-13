using Amigo.Application.Specifications;
using Amigo.Domain.Entities.TranslationEntities;

namespace Amigo.Application.Specifications.DestinationSpecification.User;


public class DestinationTranslationsByDestinationIdsSpecification : BaseSpecification<DestinationTranslation, Guid>
{
    public DestinationTranslationsByDestinationIdsSpecification(IReadOnlyCollection<Guid> destinationIds)
        : base(t => destinationIds.Contains(t.DestinationId))
    {
    }
}

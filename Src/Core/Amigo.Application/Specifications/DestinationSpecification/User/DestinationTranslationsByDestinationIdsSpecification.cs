using Amigo.Application.Specifications;
using Amigo.Domain.Entities.TranslationEntities;

namespace Amigo.Application.Specifications.DestinationSpecification.User;


public class DestinationTranslationsByDestinationIdsSpecification : UserBaseSpecification<DestinationTranslation, Guid>
{
    public DestinationTranslationsByDestinationIdsSpecification(IReadOnlyCollection<Guid> destinationIds)
        : base(t => destinationIds.Contains(t.DestinationId))
    {
    }
}

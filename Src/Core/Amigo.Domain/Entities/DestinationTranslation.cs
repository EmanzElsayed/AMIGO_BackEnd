using Amigo.Domain.Enum;
using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class DestinationTranslation : BaseEntity<Guid>
{
    public Guid DestinationId { get; set; }
    public Destination Destination { get; set; } = null!;

    public Language Language { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}


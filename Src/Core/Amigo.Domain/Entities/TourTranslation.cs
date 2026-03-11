using Amigo.Domain.Enum;
using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class TourTranslation : BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public Language Language { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string DestinationInfo { get; set; } = null!;
}


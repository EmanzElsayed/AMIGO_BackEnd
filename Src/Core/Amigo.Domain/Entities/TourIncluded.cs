using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class TourIncluded : BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;
    public string Description { get; set; } = null!;
}


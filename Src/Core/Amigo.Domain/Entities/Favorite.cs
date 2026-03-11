using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class Favorite : BaseEntity<Guid>
{
    public Guid UserId { get; set; }

    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;
}


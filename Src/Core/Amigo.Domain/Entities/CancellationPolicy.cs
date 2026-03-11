using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class CancellationPolicy : BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public int HoursBefore { get; set; }
    public int RefundPercentage { get; set; }
}


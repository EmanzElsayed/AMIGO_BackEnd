using Amigo.Domain.Enum;
using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class TourAvailability : BaseEntity<Guid>
{
    public Guid TourScheduleId { get; set; }
    public TourSchedule TourSchedule { get; set; } = null!;

    public DateOnly Date { get; set; }
    public int AvailableSpots { get; set; }
    public AvailabilityStatus Status { get; set; }
}


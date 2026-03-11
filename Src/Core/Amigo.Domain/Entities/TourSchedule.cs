using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class TourSchedule : BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public TimeSpan StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public int MaxCapacity { get; set; }
    public bool IsActive { get; set; }

    public ICollection<TourAvailability> Availabilities { get; set; } = new List<TourAvailability>();
}


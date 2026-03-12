

namespace Amigo.Domain.Entities
{
    public class AvailableSlots:BaseEntity<Guid>
    {

        public Guid TourScheduleId { get; set; }
        [Required]
        public TourSchedule TourSchedule { get; set; } = null!;

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public AvailableDateTimeStatus AvailableTimeStatus { get; set; }

        public int MaxCapacity { get; set; }

    }
}

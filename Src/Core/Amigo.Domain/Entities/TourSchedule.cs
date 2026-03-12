


namespace Amigo.Domain.Entities
{
    public class TourSchedule: BaseEntity<Guid>
    {
        public Guid TourId { get; set; }
        [Required]
        public Tour Tour { get; set; } = null!;
        public DateOnly StartDate {  get; set; }
        public DateOnly EndDate { get; set; }

        public AvailableDateTimeStatus AvailableDateStatus { get; set; }
    }
}

namespace Amigo.Domain.Entities;

[Table($"{nameof(AvailableSlots)}", Schema = SchemaConstants.tour_schema)]

public class AvailableSlots:BaseEntity<Guid>
{

    public Guid TourId { get; set; }
   
    public Tour Tour { get; set; } = null!;

    public TimeOnly StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }

    public AvailableDateTimeStatus AvailableTimeStatus { get; set; } = AvailableDateTimeStatus.Available;

    //public int MaxCapacity { get; set; }

    public int ReservedCount { get; set; }   

    public ICollection<SlotReservation> SlotReservations { get; set; } = new List<SlotReservation>();
}

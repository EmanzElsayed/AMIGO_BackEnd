namespace Amigo.Domain.Entities;

[Table($"{nameof(Booking)}", Schema = SchemaConstants.booking_schema)]

public class Booking:BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public Guid AvailableSlotsId { get; set; }
    public AvailableSlots AvailableSlots { get; set; } = null!;
    public BookingStatus Status { get; set; }
    public DateTime? BookingDate { get; set; }
    public ICollection<PeopleBooking> PeopleBookings { get; set; } = new List<PeopleBooking>(); 
}

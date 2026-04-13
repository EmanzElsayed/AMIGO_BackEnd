namespace Amigo.Domain.Entities;

[Table($"{nameof(PeopleBooking)}", Schema = SchemaConstants.booking_schema)]

public class PeopleBooking : BaseEntity<Guid>
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;


    // ✅ snapshot
    public decimal PriceAtBooking { get; set; }
    public string PriceType { get; set; } = null!;

    public int NoOfPeopleBooking { get; set; }  
    public ICollection<PeopleBookingDetails> PeopleBookingDetails { get; set; } = new List<PeopleBookingDetails>();
}

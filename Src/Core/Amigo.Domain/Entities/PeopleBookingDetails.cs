namespace Amigo.Domain.Entities;

[Table($"{nameof(PeopleBookingDetails)}", Schema = SchemaConstants.booking_schema)]

public class PeopleBookingDetails : BaseEntity<Guid>
{
    public Guid PeopleBookingId { get; set; }
    [Required]
    public PeopleBooking PeopleBooking { get; set; } = null!;
    
    [Required]
    public string FullName { get; set; } = null!;
    [Required]
    public string Nationality { get; set; } = null!; // -> or enum
                                                     
    public string? PhoneNumber { get; set; }
}

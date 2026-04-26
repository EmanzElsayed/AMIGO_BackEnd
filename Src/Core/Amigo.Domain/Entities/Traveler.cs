namespace Amigo.Domain.Entities;

[Table($"{nameof(Traveler)}", Schema = SchemaConstants.booking_schema)]

public class Traveler : BaseEntity<Guid>
{
    public Guid BookingId { get; set; }

    public Booking Booking { get; set; } 

    [Required]
    public string FullName { get; set; } = null!;

    [Required]
    public string Nationality { get; set; } = null!;


    [Required]
    public string Type { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? PassportNumber { get; set; }

    public string? PhoneNumber { get; set; }

    
}

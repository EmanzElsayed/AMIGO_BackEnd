namespace Amigo.Domain.Entities;

[Table($"{nameof(Price)}", Schema = SchemaConstants.booking_schema)]

public class Price : BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;
    public string Type { get; set; } = null!;
    public decimal Cost { get; set; }
}

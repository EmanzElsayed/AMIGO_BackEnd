namespace Amigo.Domain.Entities;

[Table($"{nameof(OrderItem)}", Schema = SchemaConstants.booking_schema)]

public class OrderItem:BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;


    public Guid? AvailableSlotsId { get; set; }
    public AvailableSlots? AvailableSlots { get; set; }
    // ✅ snapshot
    public DateOnly TourDate { get; set; }
    public TimeSpan StartTime { get; set; }


    public decimal Price { get; set; }
    public string PriceType { get; set; } = null!;

    public int Quantity { get; set; }

    public string TourTitle { get; set; } = null!;
    public string? TourDescription { get; set; }
}

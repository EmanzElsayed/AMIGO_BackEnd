namespace Amigo.Domain.Entities;

[Table($"{nameof(OrderItem)}", Schema = SchemaConstants.booking_schema)]

public class OrderItem:BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public Guid AvailableSlotsId { get; set; }
    public AvailableSlots AvailableSlots { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }

}

namespace Amigo.Domain.Entities;

[Table($"{nameof(BasketItem)}", Schema = SchemaConstants.booking_schema)]

public class BasketItem:BaseEntity<Guid>
{
    public Guid BasketId { get; set; }
    public Basket Basket { get; set; } = null!;
    public Guid AvailableSlotsId { get; set; }
    public AvailableSlots AvailableSlots { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity {  get; set; }
}

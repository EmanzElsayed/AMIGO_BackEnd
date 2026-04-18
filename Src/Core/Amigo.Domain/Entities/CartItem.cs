namespace Amigo.Domain.Entities;

[Table($"{nameof(CartItem)}", Schema = SchemaConstants.booking_schema)]

public class CartItem:BaseEntity<Guid>
{
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public Guid SlotId { get; set; }
    public AvailableSlots Slot { get; set; } = null!;

    public Language Language { get; set; }

    public DateOnly TourDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public string TourTitle { get; set; } = null!;

    public string DestinationName { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public ICollection<CartPrice> Prices { get; set; } = new List<CartPrice>();

}

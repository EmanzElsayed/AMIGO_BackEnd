namespace Amigo.Domain.Entities;

[Table($"{nameof(Order)}", Schema = SchemaConstants.booking_schema)]

public class Order :BaseEntity<Guid>
{
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public CurrencyCode Currency { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime? OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime ExpiresAt { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

}

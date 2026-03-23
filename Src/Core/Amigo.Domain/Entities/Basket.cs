namespace Amigo.Domain.Entities;

[Table($"{nameof(Basket)}", Schema = SchemaConstants.booking_schema)]

public class Basket:BaseEntity<Guid>
{
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();
}

namespace Amigo.Domain.Entities;

[Table($"{nameof(Cart)}", Schema = SchemaConstants.booking_schema)]

public class Cart:BaseEntity<Guid>
{
    public string? UserId { get; set; } 
    public ApplicationUser? User { get; set; }

    public string? CartToken { get; set; } //cookie/localStorage token

    public CurrencyCode CurrencyCode { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}

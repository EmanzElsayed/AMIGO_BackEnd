using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities;

[Table($"{nameof(CartPrice)}", Schema = SchemaConstants.booking_schema)]
public class CartPrice : BaseEntity<Guid>
{
    public Guid CartItemId { get; set; }
    public CartItem CartItem { get; set; } = null!;

    [Required]
    public string Type { get; set; } = null!;   // Adult / Child

    public decimal RetailPrice { get; set; }

    public int Quantity { get; set; }

    public decimal FinalPrice => RetailPrice * Quantity;
}


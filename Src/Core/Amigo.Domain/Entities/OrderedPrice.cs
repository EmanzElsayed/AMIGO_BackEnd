using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities;
[Table($"{nameof(OrderedPrice)}", Schema = SchemaConstants.booking_schema)]

public class OrderedPrice :BaseEntity<Guid>
{
    public Guid OrderItemId { get; set; }
    public OrderItem OrderItem { get; set; } = null!;
    
    [Required]
    public string Type { get; set; } = null!;

    public decimal BaseRetailPrice { get; set; } //price after discount
    public decimal ConvertedRetailPrice { get; set; }
    public decimal ExchangeRate { get; set; }
    public int Quantity { get; set; }

    public decimal PriceWithUsd => Quantity * BaseRetailPrice;
    public decimal FinalPrice => ConvertedRetailPrice * Quantity;
}


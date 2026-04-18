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

    public decimal RetailPrice { get; set; } //price after discount

    public int Quantity { get; set; }

    public decimal FinalPrice => RetailPrice * Quantity;
}


using Amigo.Domain.Enum;
using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class Order : BaseEntity<Guid>
{
    public Guid UserId { get; set; }

    public decimal TotalAmount { get; set; }
    public Currency Currency { get; set; }
    public OrderStatus Status { get; set; }
}


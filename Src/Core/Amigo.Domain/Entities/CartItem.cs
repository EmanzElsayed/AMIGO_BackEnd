using Amigo.Domain.Enum;
using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class CartItem : BaseEntity<Guid>
{
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public Guid? TourScheduleId { get; set; }
    public TourSchedule? TourSchedule { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public Currency Currency { get; set; }
}


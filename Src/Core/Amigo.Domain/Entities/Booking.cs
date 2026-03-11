using Amigo.Domain.Enum;
using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class Booking : BaseEntity<Guid>
{
    public Guid UserId { get; set; }

    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public Guid? TourScheduleId { get; set; }
    public TourSchedule? TourSchedule { get; set; }

    public DateTime BookingDate { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public int Infants { get; set; }

    public decimal TotalPrice { get; set; }
    public Currency Currency { get; set; }
    public BookingStatus Status { get; set; }

    public Guid? CouponId { get; set; }
    public Coupon? Coupon { get; set; }

    public Guid? PaymentId { get; set; }
    public Payment? Payment { get; set; }

    public ICollection<BookingTraveler> Travelers { get; set; } = new List<BookingTraveler>();
    public ICollection<BookingItem> Items { get; set; } = new List<BookingItem>();
}


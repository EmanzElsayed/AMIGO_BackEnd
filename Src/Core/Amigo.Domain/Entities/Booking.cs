namespace Amigo.Domain.Entities;

[Table($"{nameof(Booking)}", Schema = SchemaConstants.booking_schema)]

public class Booking:BaseEntity<Guid>
{
    public Guid OrderItemId { get; set; }
    public OrderItem OrderItem { get; set; } = null!;
    public Guid? VoucherId {  get; set; }
    public Voucher? Voucher { get; set; }
    public Guid PaymentId { get; set; }
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } 


    public string CustomerName { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;

    public string? BookingNumber { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime? ConfirmedAt { get; set; }

    public string? NameAndAddressOfAccomodation { get; set; }
    public string? CommentForProvider { get; set; }

    public bool IsVoucherCreated { get; set; } = false;

    public bool ReminderSent { get; set; } = false;
    public ICollection<Traveler> Travelers { get; set; } = new List<Traveler>(); 
}

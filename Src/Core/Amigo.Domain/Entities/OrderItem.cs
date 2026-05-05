namespace Amigo.Domain.Entities;

[Table($"{nameof(OrderItem)}", Schema = SchemaConstants.booking_schema)]

public class OrderItem:BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Booking? Booking { get; set; }

    // original refs
    public Guid? TourId { get; set; }
    public Guid? SlotId { get; set; }

    // Snapshot Data

    public string TourTitle { get; set; } = null!;
    public string DestinationName { get; set; } = null!;
    public DateOnly TourDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public CurrencyCode CurrencyCode { get; set; } 
    public Language Language { get; set; }
    public string? MeetingPoint { get; set; }
    public TimeSpan Duration { get; set; }

    public string? NameAndAddressOfAccomodation { get; set; }
    public string? CommentForProvider { get; set; }

    public string? PhoneCode { get; set; } // why you make this eman el sayed
    public string? PhoneNumber { get; set; }// why you make this eman el sayed

    // Cancellation Info :
    public CancelationPolicyType CancelationPolicyType { get; set; }
    public TimeSpan CancellationBefore { get; set; }
    public decimal RefundPercentage { get; set; }

    // price
    public ICollection<OrderedPrice> OrderedPrice = new List<OrderedPrice>();
    public ICollection<TravelerDraft> TravelersDraft { get; set; } = new List<TravelerDraft>();


}

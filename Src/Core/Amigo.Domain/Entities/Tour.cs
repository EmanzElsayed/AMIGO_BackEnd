namespace Amigo.Domain.Entities;

[Table($"{nameof(Tour)}", Schema = SchemaConstants.tour_schema)]

public class Tour: BaseEntity<Guid>
{

    public bool IsFullTime { get; set; } = false;
    public CurrencyCode CurrencyCode { get; set; } = CurrencyCode.USD;
    public TimeSpan Duration { get; set; }

    public decimal? Discount { get; set; }

    public SupportedLanguage? GuideLanguage { get; set; }

    public string? MeetingPoint { get; set; }
    
    public bool IsPitsAllowed { get; set; }

    public bool IsWheelchairAvailable { get; set; }

    public UserType UserType { get; set; }
    
    // Destination 
    public Guid DestinationId { get; set; }

    [Required]
    public Destination Destination { get; set; } = null!;


    public ICollection<TourTranslation> Translations { get; set; } = new List<TourTranslation>();



    public ICollection<TourImage> Images { get; set; } = new List<TourImage>();


    public ICollection<Price> Prices { get; set; } = new List<Price>();


    // blacout week days
    public ICollection<BlackoutWeekDay>? BlackoutWeekDays { get; set; } = new List<BlackoutWeekDay>();


    // blackout Date 

    public ICollection<BlackoutDate>? BlackoutDates { get; set; } = new List<BlackoutDate>();




    //Available Time 
    public ICollection<AvailableSlots>? AvailableTimes { get; set; } = new List<AvailableSlots>();

    //new version



    // cancelation one to one relation
    //public Cancellation? Cancellation { get; set; }


    // cancelation one to many relation
    public ICollection<Cancellation>? Cancellations { get; set; } = new List<Cancellation>();


    //Price 
    //people booking
    //tourImage
    //tourIncluded
    //tourNotIncluded


    public ICollection<TourInclusion> TourInclusions { get; set; } = new List<TourInclusion>();

    //public ICollection<TourIncluded> Included { get; set; } = new List<TourIncluded>();

    //public ICollection<TourNotIncluded> NotIncluded { get; set; } = new List<TourNotIncluded>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

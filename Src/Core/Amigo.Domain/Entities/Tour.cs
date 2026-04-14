namespace Amigo.Domain.Entities;

[Table($"{nameof(Tour)}", Schema = SchemaConstants.tour_schema)]

public class Tour: BaseEntity<Guid>
{
    
    public Currency CurrencyCode { get; set; }
    public TimeSpan Duration { get; set; }

    public decimal? Discount { get; set; }

    public Language? GuideLanguage { get; set; }

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


    //Available Time 
    public ICollection<TourSchedule> AvailableTimes { get; set; } = new List<TourSchedule>();


    // cancelation one to one relation
    public Cancellation Cancellation { get; set; } = null!;

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

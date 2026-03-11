using Amigo.Domain.Enum;
using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class Tour : BaseEntity<Guid>
{
   
    public Guid DestinationId { get; set; }
    public Destination Destination { get; set; } = null!;

    public int DurationMinutes { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public bool IsPetsAllowed { get; set; }
    public bool IsWheelchairAccessible { get; set; }
    public bool IsPrivate { get; set; }
    public bool InstantConfirmation { get; set; }
    public bool IsActive { get; set; }

    public string MeetingPoint { get; set; } = null!;
    public double MeetingPointLatitude { get; set; }
    public double MeetingPointLongitude { get; set; }

    public Currency Currency { get; set; }

    public double RatingAverage { get; set; }
    public int ReviewCount { get; set; }
    public int BookingCount { get; set; }
    public bool IsFeatured { get; set; }

    public ICollection<TourSchedule> Schedules { get; set; } = new List<TourSchedule>();
    public ICollection<TourMedia> Media { get; set; } = new List<TourMedia>();
    public ICollection<TourTranslation> Translations { get; set; } = new List<TourTranslation>();
    public ICollection<TourIncluded> IncludedItems { get; set; } = new List<TourIncluded>();
    public ICollection<TourNotIncluded> NotIncludedItems { get; set; } = new List<TourNotIncluded>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<InterestedTour> RelatedTours { get; set; } = new List<InterestedTour>();
    public ICollection<InterestedTour> RelatedFromTours { get; set; } = new List<InterestedTour>();
    public ICollection<TourPrice> Prices { get; set; } = new List<TourPrice>();
    public ICollection<TourCategory> Categories { get; set; } = new List<TourCategory>();
    public ICollection<CancellationPolicy> CancellationPolicies { get; set; } = new List<CancellationPolicy>();
}

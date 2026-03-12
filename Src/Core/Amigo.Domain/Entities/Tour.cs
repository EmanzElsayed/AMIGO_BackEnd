

using Amigo.Domain.Entities.TranslationEntities;

namespace Amigo.Domain.Entities
{
    public class Tour: BaseEntity<Guid>
    {
        public TimeSpan Duration { get; set; }

        public decimal Discount { get; set; }

        public Language GuideLanguage { get; set; }

        public string? MeetingPoint { get; set; }
        
        public bool IsPitsAllowed { get; set; }

        public bool IsWheelchairAvailable { get; set; }

        // Destination 
        public Guid DestinationId { get; set; }

        [Required]
        public Destination Destination { get; set; } = null!;



        // cancelation one to one relation
        public Cancellation Cancellation { get; set; } = null!;
        //Available Time 
        public ICollection<TourSchedule> AvailableTimes { get; set; } = new List<TourSchedule>();


        //Price 
        //people booking
        //tourImage
        //tourIncluded
        //tourNotIncluded

        public ICollection<TourTranslation> Translations { get; set; } = new List<TourTranslation>();

        public ICollection<Price> Prices { get; set; } = new List<Price>();

        public ICollection<TourImage> Images { get; set; } = new List<TourImage>();

        public ICollection<TourIncluded> Included { get; set; } = new List<TourIncluded>();

        public ICollection<TourNotIncluded> NotIncluded { get; set; } = new List<TourNotIncluded>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}

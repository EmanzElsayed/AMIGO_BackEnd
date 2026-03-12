
namespace Amigo.Domain.Entities
{
    public class PeopleBooking : BaseEntity<Guid>
    {
        public Guid BookingId { get; set; }
        [Required]
        public Booking Booking { get; set; } = null!;

        public Guid PriceId { get; set; }
        [Required]
        public Price Price { get; set; } = null!;

        public int NoOfPeopleBooking { get; set; }  

        //Total Price => calculted = Price.Cost * NoOfPeopleBooking
        public ICollection<PeopleBookingDetails> PeopleBookingDetails { get; set; } = new List<PeopleBookingDetails>();
    }
}

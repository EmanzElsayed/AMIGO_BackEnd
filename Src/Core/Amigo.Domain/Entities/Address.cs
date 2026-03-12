
namespace Amigo.Domain.Entities
{
    [Owned]
    public class Address
    {
        public int BuildingNumber { get; set; }
        [Required]
        public string City { get; set; } = null!;

        [Required]
        public string Country { get; set; } = null!;
    }
}

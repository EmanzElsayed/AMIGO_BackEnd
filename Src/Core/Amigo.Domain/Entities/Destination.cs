using System;


namespace Amigo.Domain.Entities
{
    public class Destination : BaseEntity<Guid>
    {
        public CountryCode CountryCode { get; set; }
        public bool IsActive { get; set; }

        public string? Image {  get; set; }

        public ICollection<Tour> Tours { get; set; } = new List<Tour>();
    }
}

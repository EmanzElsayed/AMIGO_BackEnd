namespace Amigo.Domain.Entities;

[Table($"{nameof(Destination)}", Schema = SchemaConstants.auth_schema)]

public class Destination : BaseEntity<Guid>
{
    public CountryCode CountryCode { get; set; }
    public bool IsActive { get; set; }
    public string? Image {  get; set; }
    public ICollection<Tour> Tours { get; set; } = new List<Tour>();
}

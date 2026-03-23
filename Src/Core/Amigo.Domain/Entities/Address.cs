namespace Amigo.Domain.Entities;

[Owned]
[Table($"{nameof(Address)}", Schema = SchemaConstants.auth_schema)]
public class Address
{
    public int BuildingNumber { get; set; }
   
    public string City { get; set; } = null!;

   
    public string Country { get; set; } = null!;
}

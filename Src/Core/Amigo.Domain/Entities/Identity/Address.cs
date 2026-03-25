namespace Amigo.Domain.Entities.Identity;

[Owned]
[Table($"{nameof(Address)}", Schema = SchemaConstants.auth_schema)]
public class Address
{
    public string? BuildingNumber { get; set; }
   
    public string? City { get; set; } 

   
    public string? Country { get; set; }
}

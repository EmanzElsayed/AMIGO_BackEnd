namespace Amigo.Domain.Entities.Identity;

[Table($"{nameof(ApplicationUser)}", Schema =SchemaConstants.auth_schema)]
public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; } 
    public bool IsActive { get; set; } = true;
    public string? Image {  get; set; }
    public Gender? Gender { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? Nationality { get; set; }
    public Language? Language { get; set; }
    public Address? Address { get; set; }
     public  DateTime LastLoginAt { get; set; }
    public int? CreatedBy { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public int? ModifiedBy { get; private set; }
    public DateTime? ModifiedDate { get; private set; }
    public bool IsDeleted { get; private set; }
    public ICollection<UserRefreshToken> UserRefreshTokens { get; set; }
    public ApplicationUser() { }

    public ApplicationUser(
        string email,
        string fullName,
        DateOnly birthDate,
        string phoneNumber,
        Gender gender,
        Language language,
        Address address,
        string userName,
        string nationality)
    {
        Email = email;
        UserName = userName;
        PhoneNumber = phoneNumber;
        FullName = fullName;
        BirthDate = birthDate;
        Gender = gender;
        Language = language;
        Address = address;
        Nationality = nationality;
    }

}


using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;


namespace Amigo.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;

        public bool IsActive { get; set; } = false;
        public string? Image {  get; set; }
        public Gender Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Nationality { get; set; } = null!;

        public Language Language { get; set; }

        public Address Address { get; set; } = null!;
        public DateTime LastLoginAt { get; set; }

        // Auditing
        public int? CreatedBy { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public int? ModifiedBy { get; private set; }
        public DateTime? ModifiedDate { get; private set; }
        public bool IsDeleted { get; private set; }

        // For EF Core and general usage
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
}

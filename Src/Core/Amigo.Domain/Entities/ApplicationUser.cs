
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;


namespace Amigo.Domain.Entities
{
    public class ApplicationUser:IdentityUser<Guid> 
    {
        // inherit from base or note 
        [Required]
        public string FullName { get; set; } = null!;

        public bool IsActive { get; set; }

        public string? Image {  get; set; }
        public Gender Gender { get; set; }

        public DateOnly BirthDate { get; set; }

        [Required]
        public string Nationality { get; set; } = null!; // -> or enum

        public Language Language { get; set; }

        //Address ->owned or seperated class


        [Required]
        public Address Address { get; set; } = null!;


        //base Info
        public int? CreatedBy { get;  set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get;  set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsDeleted { get;  set; }
    }
}

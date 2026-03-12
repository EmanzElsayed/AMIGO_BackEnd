using Amigo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Authentication
{
    public class RegisterRequestDTO
    {
        [Required(ErrorMessage = "Email Required")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "ConfirmPassword Required")]
        [Compare("Password", ErrorMessage = "Confirm password does not match password.")]
        public string ConfirmPassword { get; set; } = null!;




        [Required(ErrorMessage = "Phone Number Required")]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "User Name Required")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Full Name Required")]
        public string FullName { get; set; } = null!;


        public string? Image { get; set; }

        [Required (ErrorMessage ="Gender Required")]
        public string Gender { get; set; } = null!;

        [Required(ErrorMessage = "BirthDate Required")]

        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Nationality Required")]
        public string Nationality { get; set; } = null!;

        [Required(ErrorMessage = "Language Required")]

        public string Language { get; set; } = null!;

        //Address 

        [Required(ErrorMessage = "BuildingNumber Required")]

        public int BuildingNumber { get; set; }
        [Required (ErrorMessage ="City Required")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Country Required")]
        public string Country { get; set; } = null!;
    }
}

namespace Amigo.Domain.DTO.Authentication;

public record RegisterRequestDTO(   
    string Email,
    string Password,
    string ConfirmPassword,
    string FullName,   
    bool TermsAccepted
    
);

//in update profile

//string? FullName,
//    string? UserName = null,
//    string? PhoneNumber = null,
//    DateOnly? BirthDate = null,
//    string? Gender = null,
//    string? Language = null,
//    string? Nationality = null,
//    string? BuildingNumber = null,
//    string? City = null,
//    string? Country = null
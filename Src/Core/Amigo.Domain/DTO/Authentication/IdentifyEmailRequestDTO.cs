namespace Amigo.Domain.DTO.Authentication;

public record IdentifyEmailRequestDTO(
    string Email,
     string? FullName,
    string? CountryIsoCode,
    string? PhoneNumber
);

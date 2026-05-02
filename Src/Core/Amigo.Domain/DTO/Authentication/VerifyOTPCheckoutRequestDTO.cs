namespace Amigo.Domain.DTO.Authentication;

public record VerifyOTPCheckoutRequestDTO(
    string Email,
    string Code,
    string? FullName,
    string? ReturnUrl
);

namespace Amigo.Domain.DTO.Authentication;

public record ConfirmEmailRequest
(
    string Email,
     string Token
);

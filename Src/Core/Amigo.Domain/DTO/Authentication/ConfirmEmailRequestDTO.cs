namespace Amigo.Domain.DTO.Authentication;

public record ConfirmEmailRequestDTO
(
    string Email,
     string Token
);

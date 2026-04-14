namespace Amigo.Domain.DTO.Authentication;

public record LoginRequestDTO
(string Email , string Password, string? ReturnUrl);
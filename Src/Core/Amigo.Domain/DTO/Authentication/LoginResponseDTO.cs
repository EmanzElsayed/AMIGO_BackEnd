namespace Amigo.Domain.DTO.Authentication;

public record LoginResponseDTO
(string FullName, string Email, string Token, string Role);
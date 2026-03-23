namespace Amigo.Domain.DTO.Authentication;

public record RefreshTokenRequestDTO
(string AccessToken, string RefreshToken);
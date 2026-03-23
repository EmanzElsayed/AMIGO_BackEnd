using Amigo.Domain.DTO.Authentication;

using FluentResults;

namespace Amigo.Application.Abstraction.Services.Authentication;
public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterRequestDTO request);
    Task<Result> ConfirmEmail(ConfirmEmailRequest request);
    Task<Result<LoginResponseDTO>> LoginAsync(LoginRequestDTO request);
    Task<Result<LoginResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request);
}

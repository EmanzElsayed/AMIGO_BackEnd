using Amigo.Domain.DTO.Authentication;
using Amigo.SharedKernal.DTOs;
using Amigo.SharedKernal.DTOs.Authentication;
using FluentResults;

namespace Amigo.Application.Abstraction.Services.Authentication;
public interface IAuthService
{
    Task<Result<RegisterResponseDTO>> RegisterAsync(RegisterRequestDTO request);

    Task<Result> ConfirmEmail(ConfirmEmailRequestDTO request);

    Task<Result> ResendConfirmEmail(ResendConfrimEmailRequestDTO requestDTO);

    Task<Result<LoginResponseDTO>> LoginAsync(LoginRequestDTO request);
    //Task<Result<LoginResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request);
}

using Amigo.Application.Abstraction.Services.Authentication;
using Amigo.Domain.Abstraction.Repositories;
using Amigo.Domain.DTO.Authentication;
using Amigo.Domain.Entities.Identity;

using FluentResults;

using FluentValidation;

using Microsoft.AspNetCore.Identity;

namespace Amigo.Application.Services;

public class AuthService(UserManager<ApplicationUser> _userManager,
   IRefreshTokenRepo _refreshTokenRepo) : IAuthService
{
    public Task<Result> ConfirmEmail(ConfirmEmailRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<LoginResponseDTO>> LoginAsync(LoginRequestDTO request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<LoginResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RegisterAsync(RegisterRequestDTO request)
    {
        throw new NotImplementedException();
    }
}

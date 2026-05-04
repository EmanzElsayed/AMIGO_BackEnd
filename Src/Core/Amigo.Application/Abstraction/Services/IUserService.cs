using Amigo.Domain.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IUserService
    {
        Task<Result<LoginResponseDTO>> ContinueWithEmail(CreateAccountRequestDTO requestDTO);

        Task<Result<UserInfoResponseDTO>> GetUserProfile(string userId);
        Task<Result> UpdateUserProfile(UpdateUserProfileRequestDTO requestDTO, string userId);
    }
}

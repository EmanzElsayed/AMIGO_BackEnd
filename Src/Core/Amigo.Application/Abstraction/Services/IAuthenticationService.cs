using Amigo.Domain.DTO.Authentication;
using Amigo.SharedKernal.DTOs;
using Amigo.SharedKernal.DTOs.Authentication;

using FluentResults;

using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IAuthenticationService
    {
        Task<Result<ResultDTO<RegisterReturnDTO>>> RegisterAsync(RegisterRequestDTO request);
        //public Task<ResultDTO<RegisterReturnDTO>> RegisterAsync(RegisterRequestDTO registerRequestDTO);
        Task<Result<ResultDTO<string>>> ConfirmEmailAsync(ConfirmEmailRequestDTO confirmEmailDTO);
        Task<Result<ResultDTO<LoginReturnDTO>>> LoginAsync(LoginRequestDTO loginDTO);
    }
}

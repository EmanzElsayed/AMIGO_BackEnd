using Amigo.Domain.DTO.Authentication;
using Amigo.SharedKernal.DTOs;
using Amigo.SharedKernal.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IAuthenticationService
    {
        public Task<ResultDTO<RegisterReturnDTO>> RegisterAsync(RegisterRequestDTO registerRequestDTO);

    }
}

using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Authentication;
using Amigo.SharedKernal.DTOs.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [ApiController]

    [Route("api/auth")]
    public class AuthenticationController(IAuthenticationService _authenticationService):ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<RegisterReturnDTO>> Register(RegisterRequestDTO registerRequestDTO)
        {
            var res = await _authenticationService.RegisterAsync(registerRequestDTO);
            return Ok(res);
        }
    }
}

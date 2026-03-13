using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Authentication;
using Amigo.SharedKernal.DTOs;
using Amigo.SharedKernal.DTOs.Authentication;
using FluentResults;
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
        public async Task<ActionResult<ResultDTO<RegisterReturnDTO>>> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            var res = await _authenticationService.RegisterAsync(registerRequestDTO);
            return Ok(res);
        }
        [HttpPost("confirm-email")]
        public async Task<ActionResult<ResultDTO< string>>> ConfirmEmail([FromBody] ConfirmEmailRequestDTO confirmEmailDTO)
        {
            var res = await _authenticationService.ConfirmEmailAsync(confirmEmailDTO);
            return Ok(res);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResultDTO< LoginReturnDTO>>> Login([FromBody]LoginRequestDTO loginDTO)
        {

            var res = await _authenticationService.LoginAsync(loginDTO);
            return Ok(res);
        }
    }
}

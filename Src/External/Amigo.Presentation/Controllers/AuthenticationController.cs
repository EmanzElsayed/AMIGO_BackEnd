using Amigo.Application.Abstraction.Services;
using Amigo.Application.Abstraction.Services.Authentication;
using Amigo.Application.Mapping;
using Amigo.Domain.DTO.Authentication;
using Amigo.SharedKernal.DTOs;
using Amigo.SharedKernal.DTOs.Authentication;
using FluentResults;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [ApiController]

    [Route("api/v1/auth")]
    public class AuthenticationController(IAuthService _authService) 
        : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            var result = await _authService.RegisterAsync(registerRequestDTO);
            
            // Map Result<T> -> ResultDTO<T>

           var dto =  ResultMapping.ToResultDTO<RegisterResponseDTO>(result);

            // Choose status code
            var statusCode = result.IsSuccess
                                 ? result.Successes.FirstOrDefault()?.Metadata["StatusCode"] as int? ?? 200
                                 : result.Errors.FirstOrDefault()?.Metadata["StatusCode"] as int? ?? 400;

            return StatusCode(statusCode, dto);
            
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var result = await _authService.LoginAsync(loginRequestDTO);

            // Map Result<T> -> ResultDTO<T>

            var dto = ResultMapping.ToResultDTO<LoginResponseDTO>(result);

            // Choose status code
            var statusCode = result.IsSuccess
                                 ? result.Successes.FirstOrDefault()?.Metadata["StatusCode"] as int? ?? 200
                                 : result.Errors.FirstOrDefault()?.Metadata["StatusCode"] as int? ?? 400;

            return StatusCode(statusCode, dto);

        }

        [HttpPost("confirm-email")]
        public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailRequestDTO confirmEmailDTO)
        {
            var result = await _authService.ConfirmEmail(confirmEmailDTO);

            var dto = ResultMapping.ToResultDTO<string>(result);

            // Choose status code
            var statusCode = result.IsSuccess
                                 ? result.Successes.FirstOrDefault()?.Metadata["StatusCode"] as int? ?? 200
                                 : result.Errors.FirstOrDefault()?.Metadata["StatusCode"] as int? ?? 400;

            return StatusCode(statusCode, dto);
        }
        [HttpPost("resend-confirmation")]
        public async Task<ActionResult> ResendConfirmEmail([FromBody] ResendConfrimEmailRequestDTO requestDTO)
        {
            var result = await _authService.ResendConfirmEmail(requestDTO);

            var dto = ResultMapping.ToResultDTO<string>(result);

            // Choose status code
            var statusCode = result.IsSuccess
                                 ? result.Successes.FirstOrDefault()?.Metadata["StatusCode"] as int? ?? 200
                                 : result.Errors.FirstOrDefault()?.Metadata["StatusCode"] as int? ?? 400;

            return StatusCode(statusCode, dto);
        }


        //[HttpPost("login")]
        //public async Task<ActionResult<ResultDTO<LoginReturnDTO>>> Login([FromBody] LoginRequestDTO loginDTO)
        //{

        //    var res = await _authenticationService.LoginAsync(loginDTO);
        //    return Ok(res);
        //}
    }
}
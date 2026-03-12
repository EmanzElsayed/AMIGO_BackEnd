using Amigo.Application.Abstraction.Services;
using Amigo.Application.Validators;
using Amigo.Domain.DTO.Authentication;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Amigo.Domain.Exceptions;
using Amigo.Domain.Exceptions.AlreadyExistExceptions;
using Amigo.Domain.Extension;
using Amigo.SharedKernal.DTOs;
using Amigo.SharedKernal.DTOs.Authentication;
using FluentResults;

using FluentValidation;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Amigo.Application.Services
{
    public class AuthenticationService(UserManager<ApplicationUser> _userManager, IValidator<RegisterRequestDTO> registerValidator,
        IConfiguration _configuration,
        IEmailService _emailService) : IAuthenticationService
    {
        //public async Task<ResultDTO<RegisterReturnDTO>> RegisterAsync(RegisterRequestDTO registerRequestDTO)
        //{
        //    #region CheckGenderEnum
        //    if (!Enum.TryParse<Gender>(registerRequestDTO.Gender, true, out var gender) ||
        //                !Enum.IsDefined(typeof(Gender), gender))
        //    {
        //        var errors = new Dictionary<string, string>
        //        {
        //            { "Gender Issue", "Invalid gender value" }
        //        }; 
        //        //throw new BadRequestException(new Dictionary<string, string> { { "Gender Issue", "Invalid gender value" } });
        //        throw new BadRequestException(errors);

        //    }
        //    #endregion

        //    #region CheckGenderEnum
        //    if (!Enum.TryParse<Language>(registerRequestDTO.Language, true, out var language) ||
        //                !Enum.IsDefined(typeof(Language), language))
        //    {
        //        throw new BadRequestException(new Dictionary<string, string> { { "Language Issue", "Invalid language value" } });
        //    }
        //    #endregion

        //    bool isEmailExist = await CheckEmailExistAsync(registerRequestDTO.Email); 

        //    if (isEmailExist) throw new EmailAlreadyExistException(registerRequestDTO.Email);

            

        //    if (registerRequestDTO.UserName is null)
        //        registerRequestDTO.UserName = registerRequestDTO.Email.Split("@")[0];

        //    var MappedRegisterInfo = new ApplicationUser
        //    {
        //        Email = registerRequestDTO.Email,
        //        FullName = registerRequestDTO.FullName,
        //        BirthDate = registerRequestDTO.BirthDate,
        //        PhoneNumber = registerRequestDTO.PhoneNumber,
        //        Gender = gender,
        //        Language = language,
        //        Address = new Address
        //        {
        //            BuildingNumber = registerRequestDTO.BuildingNumber,
        //            City = registerRequestDTO.City,
        //            Country = registerRequestDTO.Country,
        //        },
        //        UserName = registerRequestDTO.UserName,
        //        Nationality = registerRequestDTO.Nationality,

        //    };

        //    var res = await _userManager.CreateAsync(MappedRegisterInfo, registerRequestDTO.Password);

        //    //check if result is correct
            
        //    if (!res.Succeeded)
        //    {
        //        var errors = res.Errors
        //            .ToDictionary(
        //                e => e.Code,
        //                e => e.Description
        //            );
        //        throw new BadRequestException(errors);
        //    }

        //    #region Add User Role 

        //    await _userManager.AddToRoleAsync(MappedRegisterInfo, "Customer");
        //    var roles = await _userManager.GetRolesAsync(MappedRegisterInfo);

        //    #endregion

        //    #region SendConfirmationEmail

        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(MappedRegisterInfo);

        //    var encodedToken = WebUtility.UrlEncode(token);

        //    var confirmLink = $"{_configuration["FrontendAPIs:ConfirmEmailFrontend"]}?email={registerRequestDTO.Email}&token={encodedToken}";

        //    await _emailService.SendEmailAsync(registerRequestDTO.Email, "Confirm your email",
        //         $@"
        //            <h3>Welcome</h3>
        //            <p>Click the link below to activate your account:</p>
        //            <a href='{confirmLink}'>Confirm Email</a>
        //            "
        //         );

        //    #endregion

        //    var data = new RegisterReturnDTO()
        //    {
        //        FullName = registerRequestDTO.FullName,
        //        Email = registerRequestDTO.Email,
        //        Role = roles[0]

        //    };

        //    return new ResultDTO<RegisterReturnDTO>
        //    (
        //        Data : data,
        //        StatusCode : (int)HttpStatusCode.Created,
        //        Messages : new Dictionary<string, string>
        //        {
        //             { "ConfirmEmail","Registration successful. Please confirm your email using the link sent to  your inbox"}
        //        }
        //    );

        //}

        //private async Task<bool> CheckEmailExistAsync(string email)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user is not null && user.EmailConfirmed == true) return true;
        //    else return false;
        //}
        public async Task<Result> RegisterAsync(RegisterRequestDTO request)
        {

            
            var validation = await registerValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .Select(g => $"{g.Key}: {string.Join(", ", g.Select(e => e.ErrorMessage))}")
                    .ToList();
                return Result.Fail(errors);
            }

            if (!Enum.TryParse<Gender>(request.Gender, true, out var gender) ||
                !Enum.IsDefined(typeof(Gender), gender))
            {
                return Result.Fail("Invalid gender value");
            }
            if (!Enum.TryParse<Language>(request.Language, true, out var language) ||
                !Enum.IsDefined(typeof(Language), language))
            {
                return Result.Fail("Invalid language value");
            }
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null && existingUser.EmailConfirmed)
            {
                return Result.Fail($"Email '{request.Email}' is already registered.");
            }
            var userName = request.UserName;
            if (string.IsNullOrWhiteSpace(userName))
                userName = request.Email.Split('@')[0];

            var user = new ApplicationUser(
                request.Email,
                request.FullName,
                request.BirthDate,
                request.PhoneNumber,
                gender,
                language,
                new Address
                {
                    BuildingNumber = request.BuildingNumber,
                    City = request.City,
                    Country = request.Country,
                },
                request.UserName,
                request.Nationality
            );

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors
                    .Select(e => $"{e.Code}: {e.Description}")
                    .ToList();
                return Result.Fail(errors);
            }
            await _userManager.AddToRoleAsync(user, "Customer");
            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? "Customer";
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var confirmLink =
                $"{_configuration["FrontendAPIs:ConfirmEmailFrontend"]}?email={request.Email}&token={encodedToken}";
            await _emailService.SendEmailAsync(
                request.Email,
                "Confirm your email",
                $"""
            <h3>Welcome</h3>
            <p>Click the link below to activate your account:</p>
            <a href='{confirmLink}'>Confirm Email</a>
            """
            );
            
            return Result.Ok().WithSuccess("Registration successful. Please confirm your email.");

        }
    }

}

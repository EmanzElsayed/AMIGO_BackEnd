using Amigo.Application.Abstraction;
using Amigo.Application.Abstraction.Services.Authentication;
using Amigo.Application.Mapping;
using Amigo.Application.Validators;
using Amigo.Domain.Abstraction.Repositories;
using Amigo.Domain.DTO.Authentication;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.Identity;
using Amigo.Domain.Enum;
using Amigo.Domain.Errors;
using Amigo.Domain.Errors.BusinessErrors;
using Amigo.Domain.Extension;
using Amigo.SharedKernal.DTOs;
using Amigo.SharedKernal.DTOs.Authentication;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace Amigo.Application.Services;

public class AuthService(
                         UserManager<ApplicationUser> _userManager,
                         IRefreshTokenRepo _refreshTokenRepo,

                         IValidator<RegisterRequestDTO> registerValidator,
                         IValidator<ConfirmEmailRequestDTO> confirmEmailValidator,
                         IValidator<LoginRequestDTO> loginValidator,
                         IValidator<ResendConfrimEmailRequestDTO> resendConfirmEmailValidator,
                         IValidator<ForgetPasswordRequestDTO> forgetPasswordValidator,
                         IValidator<ResetPasswordRequestDTO> resetPasswordValidator,
                         IConfiguration _configuration,
                         IEmailService _emailService,
                         IUserMapping _userMapping
    ) : IAuthService
{
    

    public async Task<Result<LoginResponseDTO>> LoginAsync(LoginRequestDTO requestDTO)
    {
        var validationResult = await loginValidator.ValidateAndGroupErrorsAsync(requestDTO);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var user = await _userManager.FindByEmailAsync(requestDTO.Email);

        if (user is null)
        {
            return Result.Fail(new Error("Invalid Email Or Password")
                                .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

        }

        if (!user.EmailConfirmed)
        {
            var SendEmailResult = await SendConfirmEmail(user);
            if (!SendEmailResult.IsSuccess)
            {
                return SendEmailResult;
            }
            return Result.Fail(new Error("Please Confirm Your Email First")
                                .WithMetadata("StatusCode", HttpStatusCode.BadRequest));


        }
        // after sms 
        //if (!user.PhoneNumberConfirmed)
        //{
        //    return Result.Fail($"Please Confirm Your Phone Number First!!");

        //}
        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, requestDTO.Password);
        if (isPasswordCorrect)
        {
            string role = await GetRole(user);
            var data = new LoginResponseDTO
            (
                Email: requestDTO.Email,
                Token: await GenerateToken(user),
                Role: role
            );

            return Result.Ok(data)
                 .WithSuccess(new Success("Welcome To Amigo Arabe Tours")
                 .WithMetadata("StatusCode", HttpStatusCode.OK));

        }
        else
        {
            return Result.Fail(new Error("Invalid Email Or Password")
                                  .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

        }
    }

    public Task<Result<LoginResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> ForgetPassword(ForgetPasswordRequestDTO requestDTO)
    {
        // Use the extension method
        Result validationResult = await forgetPasswordValidator.ValidateAndGroupErrorsAsync(requestDTO);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }
        var user = await _userManager.FindByEmailAsync(requestDTO.Email);

        if (!IsEmailExist(user))
        {
            return Result.Fail(new Error($"This Email '{requestDTO.Email}' Not Found")
                               .WithMetadata("StatusCode", HttpStatusCode.NotFound));

        }
        var resetEmailResult =  await SendResetPasswordEmail(user);
        if(!resetEmailResult.IsSuccess)
        {
            return resetEmailResult;
        }

        return Result.Ok()
           .WithSuccess(new Success("If an account is associated with this email, you’ll receive a password reset link.")
           .WithMetadata("StatusCode", HttpStatusCode.OK));
    }

    public async Task<Result> ResetPassword(ResetPasswordRequestDTO requestDTO)
    {
        // Use the extension method
        Result validationResult = await resetPasswordValidator.ValidateAndGroupErrorsAsync(requestDTO);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }
        var user = await _userManager.FindByEmailAsync(requestDTO.Email);

        if (!IsEmailExist(user))
        {
            return Result.Fail(new Error($"This Email '{requestDTO.Email}' Not Found")
                               .WithMetadata("StatusCode", HttpStatusCode.NotFound));

        }

        var token = WebUtility.UrlDecode(requestDTO.Token);
       
        var result = await _userManager.ResetPasswordAsync(user, token, requestDTO.NewPassword);

        if (!result.Succeeded)
        {
            return FluentValidationExtension.FromIdentityErrors(result.Errors);
        }

        return Result.Ok()
          .WithSuccess(new Success("Password Changed SuccessFully, Please Login")
          .WithMetadata("StatusCode", HttpStatusCode.OK));

    }
    public async Task<Result<RegisterResponseDTO>> RegisterAsync(RegisterRequestDTO requestDTO)
    {

        // Use the extension method
        Result validationResult = await registerValidator.ValidateAndGroupErrorsAsync(requestDTO);
        if (!validationResult.IsSuccess)
        { 
            return validationResult;
        }

        var existingUser = await _userManager.FindByEmailAsync(requestDTO.Email);

        #region Check Email
        if ( IsEmailExist(existingUser))
        {
            return Result.Fail(new EmailAlreadyExistsError(requestDTO.Email)
                            .WithMetadata("StatusCode",(int)HttpStatusCode.Conflict));
          
        }

        #endregion

        #region Check Exist Email And It is not Confirmed And Resing Email

        var ExistEmailAndNotConfirmedResponse =  await IsEmailExistAndNotConfirmedAndResingEmail(existingUser);
        if (ExistEmailAndNotConfirmedResponse is not null)
        {
            return ExistEmailAndNotConfirmedResponse;
        }
        #endregion

        var user = _userMapping.ToEntity(requestDTO);

        var createResult = await _userManager.CreateAsync(user, requestDTO.Password);

        if (!createResult.Succeeded)
        {
            return FluentValidationExtension.FromIdentityErrors(createResult.Errors);
        }
        await _userManager.AddToRoleAsync(user, "Customer");

        // Get Role 
        var role = await GetRole(user);
       

        var SendEmailResult =   await SendConfirmEmail(user);
        if (!SendEmailResult.IsSuccess)
        { 
            return SendEmailResult;
        }

        var result = new RegisterResponseDTO
        (
            
                user.Email,
                role
        );

        return Result.Ok(result)
            .WithSuccess(new Success("Registration successful. Please confirm your email using the link sent to your inbox")
            .WithMetadata("StatusCode", HttpStatusCode.Created));
            

    }


    public async Task<Result> ConfirmEmail(ConfirmEmailRequestDTO requestDTO)
    {
        
        // Use the extension method
        Result validationResult = await confirmEmailValidator.ValidateAndGroupErrorsAsync(requestDTO);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }
        var user = await _userManager.FindByEmailAsync(requestDTO.Email);

        if (user is null)
        {
            return Result.Fail(new Error($"This Email '{requestDTO.Email}' Not Found")
                                .WithMetadata("StatusCode",HttpStatusCode.NotFound));

        }

        if (IsEmailExist(user))
        {
            return Result.Ok()
                       .WithSuccess(new Success("Email Already Confirmed Please Login")
                       .WithMetadata("StatusCode", HttpStatusCode.OK));
        }

        var token = WebUtility.UrlDecode(requestDTO.Token);

        var result = await _userManager.ConfirmEmailAsync(user, token);


        if (!result.Succeeded)
        {
            return FluentValidationExtension.FromIdentityErrors(result.Errors);
        }

        return Result.Ok()
            .WithSuccess(new Success("Email Confirmed SuccessFully, Please Login")
            .WithMetadata("StatusCode", HttpStatusCode.OK));

    }



    public async Task<Result> ResendConfirmEmail(ResendConfrimEmailRequestDTO requestDTO)
    {
        // Use the extension method
        Result validationResult = await resendConfirmEmailValidator.ValidateAndGroupErrorsAsync(requestDTO);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }
        var user = await _userManager.FindByEmailAsync(requestDTO.Email);

        if (user is null)
        {
            return Result.Fail(new Error($"This Email '{requestDTO.Email}' Not Found")
                                .WithMetadata("StatusCode", HttpStatusCode.NotFound));

        }

        if (IsEmailExist(user))
        {
            return Result.Ok()
                       .WithSuccess(new Success("Email Already Confirmed Please Login")
                       .WithMetadata("StatusCode", HttpStatusCode.OK));
        }

        if (user is not null && !user.EmailConfirmed)
        {
            var SendEmailResult = await SendConfirmEmail(user);
            if (!SendEmailResult.IsSuccess)
            {
                return SendEmailResult;
            }
        }

        return Result.Ok()
            .WithSuccess(new Success("Please confirm your email using the link sent to your inbox")
            .WithMetadata("StatusCode", HttpStatusCode.OK));
    }


    private bool IsEmailExist(ApplicationUser? existingUser)
    {
        if (existingUser is null) return false;
        if (existingUser is not null && existingUser.EmailConfirmed)
        {
            return true;
        }
        return false;
    }

    private async Task<string> GetRole(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var primaryRole = roles.FirstOrDefault() ?? "Customer";
        return primaryRole;

    }
    private async Task<Result<RegisterResponseDTO>?> IsEmailExistAndNotConfirmedAndResingEmail(ApplicationUser? existingUser)
    {
        if (existingUser is null) return null;

        if (existingUser is not null && !existingUser.EmailConfirmed)
        {
            await SendConfirmEmail(existingUser);

            string role = await GetRole(existingUser);
            var response = new RegisterResponseDTO(existingUser.Email, role);


            return Result.Ok(response).WithSuccess(new Success("Email already exists but not confirmed. Please check your email.")
            .WithMetadata("StatusCode", HttpStatusCode.OK));

        }
        else return null;

    }
    private async Task<Result> SendConfirmEmail(ApplicationUser user)
    {
        try
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var confirmLink =
                $"{_configuration["FrontendAPIs:ConfirmEmailFrontend"]}?confirmemail={user.Email}&token={encodedToken}";
            await _emailService.SendEmailAsync(
                user.Email,
                "Confirm your email",
                $"""
                    <h3>Welcome</h3>
                    <p>Click the link below to activate your account:</p>
                    <a href='{confirmLink}'>Confirm Email</a>
                    """
            );

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return FluentValidationExtension.FromException(details: ex.Message);
        }

    }

    private async Task<Result> SendResetPasswordEmail(ApplicationUser user)
    {
        try
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var resetPasswordLink =
                $"{_configuration["FrontendAPIs:ResetPasswordFrontend"]}?email={user.Email}&token={encodedToken}";
            await _emailService.SendEmailAsync(
                user.Email,
                "Reset Password",
                $"""
                    <h3>Welcome</h3>
                    <p>Click the link below to reset your password:</p>
                    <a href='{resetPasswordLink}'>Reset Password Email</a>
                    """
            );

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return FluentValidationExtension.FromException(details: ex.Message);
        }

    }
    private async Task<string> GenerateToken(ApplicationUser User)
    {
        // header 
        var secretKey = _configuration["JWTOptions:SecretKey"];

        var EncodedSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var Creds = new SigningCredentials(EncodedSecurityKey, SecurityAlgorithms.HmacSha256);


        //paylodad
        //
        var UserClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email,User.Email),
                new Claim(ClaimTypes.Name , User.UserName),
                new Claim(ClaimTypes.NameIdentifier,User.Id)
            };
        var Roles = await _userManager.GetRolesAsync(User);
        foreach (var role in Roles)
        {
            UserClaims.Add(new Claim(ClaimTypes.Role, role));
        }



        //signeture

        var token = new JwtSecurityToken
        (
            issuer: _configuration["JWTOptions:Issuer"],
            audience: _configuration["JWTOptions:Audience"],
            expires: DateTime.Now.AddDays(2),
            claims: UserClaims,
            signingCredentials: Creds

        );
        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    
}


using Amigo.Application.Specifications.CustomerSpecification;
using Amigo.Domain.DTO.Customer;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class CustomerService(IValidationService _validationService,
                                    IUserRepo _userRepo,
                                    IConfiguration _configuration,
                                    IJWTTokenService _jWTTokenService,
                                    UserManager<ApplicationUser> _userManager, IEmailService _emailService) 
                                                    : ICustomerService
    {
        private readonly PhoneNumberUtil _phoneUtil  = PhoneNumberUtil.GetInstance();

        public async Task<Result<LoginResponseDTO>> ContinueWithEmail(CreateAccountRequestDTO requestDTO)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }


            var user = await _userRepo.GetByIdAsync(new GetUserWithEmailConfirmedSpecification(requestDTO.Email));

            if (user is not null)
            {
                user.PhoneNumber = FormatPhone(requestDTO.PhoneNumber, requestDTO.CountryIsoCode);
                await _userManager.UpdateAsync(user);
                if (!user.EmailConfirmed)
                { 

                    var SendEmailResult = await SendConfirmEmailAsync(user,requestDTO.ReturnUrl);
                    if (!SendEmailResult.IsSuccess)
                    {
                        return SendEmailResult;
                    }

                    return Result.Ok()
                        .WithSuccess(new Success("Please confirm your email using the link sent to your inbox"));

                }

                return Result.Ok(await BuildLoginResponse(user));
               
            }
            // create new user
            var newUser = new ApplicationUser
            {
                UserName = requestDTO.Email,
                Email = requestDTO.Email,
                FullName = requestDTO.FirstName + " " + requestDTO.LastName,
                PhoneNumber = FormatPhone(requestDTO.PhoneNumber, requestDTO.CountryIsoCode)
            };

            var createResult = await _userManager.CreateAsync(newUser);

            if (!createResult.Succeeded)
                return Result.Fail("Cannot create account");

            var result = await SendConfirmEmailAsync(newUser , requestDTO.ReturnUrl);
            if (!result.IsSuccess)
            {
                return result;
            }

            return Result.Ok()
                .WithSuccess(new Success("Please confirm your email using the link sent to your inbox"));


        }
        private async Task<LoginResponseDTO> BuildLoginResponse(ApplicationUser user)
        {
            return new LoginResponseDTO(
                FullName: user.FullName,
                Email: user.Email,
                AccessToken: await _jWTTokenService.GenerateToken(user),
                RefreshToken: _jWTTokenService.GenerateRefreshToken(),
                AccessTokenExpiresIn: DateTime.UtcNow.AddDays(1),
                EmailConfirmed: user.EmailConfirmed,
                Role: null
            );
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
        private async Task<Result> SendConfirmEmailAsync(ApplicationUser user, string? returnUrl = null)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(token);
                var confirmLink = $"{_configuration["FrontendAPIs:ConfirmEmailFrontend"]}?confirmemail={user.Email}&token={encodedToken}";
                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    confirmLink += $"&returnUrl={WebUtility.UrlEncode(returnUrl)}";
                }
                Console.WriteLine("confirm link: " + confirmLink);
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

        private string FormatPhone(string phone, string region)
        {
            var number = _phoneUtil.Parse(phone, region);
            return _phoneUtil.Format(number, PhoneNumberFormat.E164);
        }

    }
}

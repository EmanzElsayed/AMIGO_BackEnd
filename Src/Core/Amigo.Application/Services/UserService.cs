using Amigo.Application.Specifications.UserSpecification;
using Amigo.Domain.DTO.User;
using Amigo.Domain.Entities;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class UserService(IValidationService _validationService,
                                    IUserRepo _userRepo,
                                    IConfiguration _configuration,
                                    IJWTTokenService _jWTTokenService,
                                    UserManager<ApplicationUser> _userManager,
                                    IEmailService _emailService,
                                    ImageCloudService _imageCloud,
                                    IUnitOfWork _unitOfWork
                                    ) 
                                                    : IUserService
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


        public async Task<Result<UserInfoResponseDTO>> GetUserProfile(string userId)
        {
            var user = await _userRepo.GetByIdAsync(new GetUserByIdSpecification(userId));
            if (user is null)
            {
                return Result.Fail(new NotFoundError("Your Account is Missing"));

            }
            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? "Public";

            var mappedUser = user.ToUserDTO(primaryRole);

            return Result.Ok(mappedUser);

        }
        public async Task<Result> UpdateUserProfile(UpdateUserProfileRequestDTO requestDTO, string userId)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var user = await _userRepo.GetByIdAsync(new GetUserByIdSpecification(userId));
            if (user is null)
            {
                return Result.Fail(new NotFoundError("Your Account is Missing"));

            }
            if (user.Address is null)
            {
                user.Address = new Address();
            }
            if(!string.IsNullOrWhiteSpace(requestDTO.FullName)) user.FullName = requestDTO.FullName;
            if (!string.IsNullOrWhiteSpace(requestDTO.Nationality)) user.Nationality = requestDTO.Nationality;
            if (!string.IsNullOrWhiteSpace(requestDTO.BuildingNumber)) user.Address.BuildingNumber = requestDTO.BuildingNumber;
            if (!string.IsNullOrWhiteSpace(requestDTO.City)) user.Address.City = requestDTO.City;
            if (!string.IsNullOrWhiteSpace(requestDTO.Country)) user.Address.Country = requestDTO.Country;

            if (!string.IsNullOrWhiteSpace(requestDTO.Language)) user.Language = EnumsMapping.ToLanguageEnum(requestDTO.Language);
            if (!string.IsNullOrWhiteSpace(requestDTO.Gender)) user.Gender = EnumsMapping.ToEnum<Gender>(requestDTO.Gender,false);
            
            //create validate for birth date
            if (requestDTO.BirthDate is not null) user.BirthDate = requestDTO.BirthDate;

            if (!string.IsNullOrWhiteSpace(requestDTO.PhoneNumber) && !string.IsNullOrWhiteSpace(requestDTO.CountryIsoCode))
            {
                user.PhoneNumber = FormatPhone(requestDTO.PhoneNumber, requestDTO.CountryIsoCode);

            }
            if (requestDTO.ImageUrl is not null)
            {
                user.ImageUrl = requestDTO.ImageUrl;

                if (user.ImagePublicId is not null)
                    _imageCloud.DeleteImage(user.ImagePublicId);

                if (requestDTO.ImagePublicId is not null)
                    user.ImagePublicId = requestDTO.ImagePublicId;
            }

            try {
                await _unitOfWork.SaveChangesAsync();
                return Result.Ok()
                                 .WithSuccess(new Success("Your Profile Updated Successfully"));
            }
            catch (Exception ex)
            {
                return FluentValidationExtension.FromException(details: ex.Message);

            }

           
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

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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Amigo.Application.Services
{
    public class AuthenticationService(UserManager<ApplicationUser> _userManager,
        IValidator<RegisterRequestDTO> registerValidator,
        IValidator<ConfirmEmailRequestDTO> confirmEmailValidator,
        IValidator<LoginRequestDTO> loginValidator,

        IConfiguration _configuration,
        IEmailService _emailService) : IAuthenticationService
    {
        
        public async Task<Result<ResultDTO<RegisterReturnDTO>>> RegisterAsync(RegisterRequestDTO request)
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
            #region Check Email
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser is not null && existingUser.EmailConfirmed)
            {
                return Result.Fail($"Email '{request.Email}' is already registered.");
            }
            if (existingUser is not null && !existingUser.EmailConfirmed) // background service later
            {
                await _userManager.DeleteAsync(existingUser);
            }
            #endregion

            #region Check Phone
            var existingPhoneNumber = await _userManager.Users
                                                        .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (existingPhoneNumber is not null /*&& existingPhoneNumber.PhoneNumberConfirmed => after add sms */)
            {
                return Result.Fail($"PhoneNumber '{request.PhoneNumber}' is already registered.");

            }
            //After SMS
            //if (existingPhoneNumber is not null && !existingPhoneNumber.EmailConfirmed) // background service later
            //{
            //    await _userManager.DeleteAsync(existingPhoneNumber);
            //}
            #endregion

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

            var data = new RegisterReturnDTO

                (
                request.FullName,
                request.Email,
                roles[0]
                );
            

            return new ResultDTO<RegisterReturnDTO>
            (
                Data: data,
                StatusCode: (int)HttpStatusCode.Created,
                Messages: new Dictionary<string, string>
                {
                         { "ConfirmEmail","Registration successful. Please confirm your email using the link sent to  your inbox"}
                }
            );

            //return Result.Ok().WithSuccess("Registration successful. Please confirm your email.");

        }

        public async Task<Result<ResultDTO<string>>> ConfirmEmailAsync(ConfirmEmailRequestDTO confirmEmailDTO)
        {
            var validation = await  confirmEmailValidator.ValidateAsync(confirmEmailDTO);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .Select(g => $"{g.Key}: {string.Join(", ", g.Select(e => e.ErrorMessage))}")
                    .ToList();
                return Result.Fail(errors);
            }
            var user = await _userManager.FindByEmailAsync(confirmEmailDTO.Email);

            if (user is null)
            {
                return Result.Fail($"This Email '{confirmEmailDTO.Email}' Not Found");

            }
            

            var token = WebUtility.UrlDecode(confirmEmailDTO.Token);

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => $"{e.Code}: {e.Description}")
                    .ToList();
                return Result.Fail(errors);

            }
            else
            {
                return new ResultDTO<string>
                    (
                        Data : "Confirmation Success",
                        Messages: new Dictionary<string, string>
                        {
                            { "ConfirmEmail","Email Confirmed Successfully Pleas Login"}
                        }
                    );
                
            }
        }

        public async Task<Result<ResultDTO<LoginReturnDTO>>> LoginAsync(LoginRequestDTO loginDTO)
        {
            var validation = await loginValidator.ValidateAsync(loginDTO);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .Select(g => $"{g.Key}: {string.Join(", ", g.Select(e => e.ErrorMessage))}")
                    .ToList();
                return Result.Fail(errors);
            }

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user is null) {
                return Result.Fail($"This Email '{loginDTO.Email}' Not Found");

            }

            if (!user.EmailConfirmed)
            {
                return Result.Fail($"Please Confirm Your Email First!!");

            }
            // after sms 
            //if (!user.PhoneNumberConfirmed)
            //{
            //    return Result.Fail($"Please Confirm Your Phone Number First!!");

            //}
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (isPasswordCorrect)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var data = new LoginReturnDTO
                (
                    FullName: user.FullName,
                    Email: loginDTO.Email,
                    Token: await GenerateToken(user),
                    Role: roles[0]
                );
                return new ResultDTO<LoginReturnDTO>
               (
                    Data: data
                );
            }
            else {
                return Result.Fail($"Unauhterize");
                
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
     
}

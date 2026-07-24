

using Amigo.Application.BackgroundTasks;
using Amigo.Application.Specifications.Identity;
using Amigo.Domain.DTO.Authentication;
using Amigo.Domain.Entities.Identity;
using Amigo.Domain.Enum;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using PhoneNumbers;
using System.Net;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Amigo.Application.Services;

public class AuthService(
                         UserManager<ApplicationUser> _userManager,
                         IRefreshTokenRepo _refreshTokenRepo,

                        IValidationService _validationService,
                         IConfiguration _configuration,
                         IEmailService _emailService,
                         IUnitOfWork _unitOfWork,
                         ILocalizationService _localizationService,
                         IBackgroundTaskQueue _backgroundTaskQueue,
                         IJWTTokenService _jWTTokenService
    ) : IAuthService
{

    private readonly PhoneNumberUtil _phoneUtil = PhoneNumberUtil.GetInstance();

    public async Task<Result<LoginResponseDTO>> LoginAsync(LoginRequestDTO requestDTO, CancellationToken cancellationToken)
    {


        var validationResult = await _validationService.ValidateAsync(requestDTO);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var user = await _userManager.FindByEmailAsync(requestDTO.Email);

        if (user is null)
        {
            return Result.Fail(new UnauthorizedError("Auth_InvalidCredentials"));


        }
        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, requestDTO.Password);

        if (!isPasswordCorrect)
            return Result.Fail(new UnauthorizedError("Auth_InvalidCredentials"));




        string role = await GetRole(user);

        var data = new LoginResponseDTO
        (
            FullName: user.FullName ?? user.UserName,
            Email: requestDTO.Email,
            AccessToken: await _jWTTokenService.GenerateToken(user),
            RefreshToken: _jWTTokenService.GenerateRefreshToken(),
            AccessTokenExpiresIn: DateTime.UtcNow.AddDays(1),
            Role: role,
            EmailConfirmed: user.EmailConfirmed
        );

        var refreshToken = new UserRefreshToken()
        {
            RefreshToken = data.RefreshToken,
            UserId = user.Id,
            User = user,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(15),

        };

        try
        {
            await _refreshTokenRepo.AddToken(refreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            return FluentValidationExtension.FromException(details: ex.Message);

        }
        // send confirm email

        if (!user.EmailConfirmed)
        {
            var userId = user.Id;
            var returnUrl = requestDTO.ReturnUrl;

            await _backgroundTaskQueue.EnqueueAsync(async (serviceProvider, cancellationToken) =>
            {

                using var scope = serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                var backgroundUser = await userManager.FindByIdAsync(userId.ToString());
                if (backgroundUser != null)
                {
                    await SendConfirmEmailScoped(backgroundUser, userManager, emailService, configuration, returnUrl);
                }
            });
        }

        return Result.Ok(data)
                .WithSuccess(new Success(_localizationService.Get("GreetingToAmigo")));


    }



    public async Task<Result> ForgetPassword(ForgetPasswordRequestDTO requestDTO)
    {
        // Use the extension method
        var validationResult = await _validationService.ValidateAsync(requestDTO);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }
        var user = await _userManager.FindByEmailAsync(requestDTO.Email);

        if (user is null)
        {
            return Result.Fail(new NotFoundEmailError(requestDTO.Email));
        }

        if (!user.EmailConfirmed)
        {
            return Result.Fail(new EmailNotConfirmedError(requestDTO.Email));
        }

        var userId = user.Id;

        await _backgroundTaskQueue.EnqueueAsync(async (serviceProvider, cancellationToken) =>
        {
            using var scope = serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var backgroundUser = await userManager.FindByIdAsync(userId.ToString());
            if (backgroundUser != null)
            {
                await SendResetPasswordEmailScoped(backgroundUser, userManager, emailService, configuration);
            }
        });

        return Result.Ok()
           .WithSuccess(new Success("If an account is associated with this email, you’ll receive a password reset link."));
    }

    public async Task<Result> ResetPassword(ResetPasswordRequestDTO requestDTO)
    {
        // Use the extension method
        var validationResult = await _validationService.ValidateAsync(requestDTO);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }
        var user = await _userManager.FindByEmailAsync(requestDTO.Email);

        if (!IsEmailExist(user))
        {
            return Result.Fail(new NotFoundEmailError(requestDTO.Email));


        }

        var result = await _userManager.ResetPasswordAsync(user, requestDTO.Token, requestDTO.NewPassword);

        if (!result.Succeeded)
        {
            return FluentValidationExtension.FromIdentityErrors(result.Errors);
        }

        return Result.Ok()
          .WithSuccess(new Success("Password Changed SuccessFully, Please Login"));


    }
    public async Task<Result> RegisterAsync(RegisterRequestDTO requestDTO)
    {

        // Use the extension method
        var validationResult = await _validationService.ValidateAsync(requestDTO);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var existingUser = await _userManager.FindByEmailAsync(requestDTO.Email);

        #region Check Email
        if (IsEmailExist(existingUser))
        {
            return Result.Fail(new EmailAlreadyExistsError(requestDTO.Email));


        }

        #endregion

        #region Check Exist Email And It is not Confirmed And Resing Email

        var ExistEmailAndNotConfirmedResponse = await IsEmailExistAndNotConfirmedAndResingEmail(existingUser, requestDTO.ReturnUrl);
        if (ExistEmailAndNotConfirmedResponse is not null)
        {
            return ExistEmailAndNotConfirmedResponse;
        }
        #endregion

        var user = requestDTO.ToEntity();

        var createResult = await _userManager.CreateAsync(user, requestDTO.Password);

        if (!createResult.Succeeded)
        {
            return FluentValidationExtension.FromIdentityErrors(createResult.Errors);
        }
        await _userManager.AddToRoleAsync(user, "Public");


        var userId = user.Id;
        var returnUrl = requestDTO.ReturnUrl;



        await _backgroundTaskQueue.EnqueueAsync(async (serviceProvider, cancellationToken) =>
        {

            using var scope = serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var backgroundUser = await userManager.FindByIdAsync(userId.ToString());
            if (backgroundUser != null)
            {
                await SendConfirmEmailScoped(backgroundUser, userManager, emailService, configuration, returnUrl);
            }
        });

        return Result.Ok()
            .WithSuccess(new Success("Registration successful. Please confirm your email using the link sent to your inbox")
            .WithMetadata("StatusCode", (int)HttpStatusCode.Created));



    }


    public async Task<Result> ConfirmEmail(ConfirmEmailRequestDTO requestDTO)
    {

        // Use the extension method
        var validationResult = await _validationService.ValidateAsync(requestDTO);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }
        var user = await _userManager.FindByEmailAsync(requestDTO.Email);

        if (user is null)
        {
            return Result.Fail(new NotFoundEmailError(requestDTO.Email));


        }

        if (IsEmailExist(user))
        {
            return Result.Ok()
                       .WithSuccess(new Success("Email Already Confirmed Please Login"));

        }

        var result = await _userManager.ConfirmEmailAsync(user, requestDTO.Token);


        if (!result.Succeeded)
        {
            return FluentValidationExtension.FromIdentityErrors(result.Errors);
        }

        return Result.Ok()
            .WithSuccess(new Success("Email Confirmed SuccessFully, Please Login"));


    }



    public async Task<Result> ResendConfirmEmail(ResendConfrimEmailRequestDTO requestDTO)
    {
        // Use the extension method
        var validationResult = await _validationService.ValidateAsync(requestDTO);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }
        var user = await _userManager.FindByEmailAsync(requestDTO.Email);

        if (user is null)
        {
            return Result.Fail(new NotFoundEmailError(requestDTO.Email));


        }

        if (IsEmailExist(user))
        {
            return Result.Ok()
                       .WithSuccess(new Success("Email Already Confirmed Please Login"));

        }

        if (user is not null && !user.EmailConfirmed)
        {
            var userId = user.Id;
            var returnUrl = requestDTO.ReturnUrl;

            await _backgroundTaskQueue.EnqueueAsync(async (serviceProvider, cancellationToken) =>
            {

                using var scope = serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                var backgroundUser = await userManager.FindByIdAsync(userId.ToString());
                if (backgroundUser != null)
                {
                    await SendConfirmEmailScoped(backgroundUser, userManager, emailService, configuration, returnUrl);
                }
            });
        }

        return Result.Ok()
            .WithSuccess(new Success("Please confirm your email using the link sent to your inbox"));

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
        var primaryRole = roles.FirstOrDefault() ?? "Public";
        return primaryRole;

    }
    private async Task<Result?> IsEmailExistAndNotConfirmedAndResingEmail(ApplicationUser? existingUser, string? returnUrl = null)
    {
        if (existingUser is null) return null;

        if (existingUser is not null && !existingUser.EmailConfirmed)
        {
            var userId = existingUser.Id;



            await _backgroundTaskQueue.EnqueueAsync(async (serviceProvider, cancellationToken) =>
            {

                using var scope = serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                var backgroundUser = await userManager.FindByIdAsync(userId.ToString());
                if (backgroundUser != null)
                {
                    await SendConfirmEmailScoped(backgroundUser, userManager, emailService, configuration, returnUrl);
                }
            });


            return Result.Ok()
                .WithSuccess(new Success("Email already exists but not confirmed. Please check your email.")
            );

        }
        else return null;

    }
    private async Task<Result> SendConfirmEmailScoped(
            ApplicationUser user,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IConfiguration configuration,
            string? returnUrl = null)
    {
        try
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var confirmLink = $"{configuration["FrontendAPIs:ConfirmEmailFrontend"]}?email={user.Email}&token={encodedToken}";

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                confirmLink += $"&returnUrl={WebUtility.UrlEncode(returnUrl)}";
            }

            Console.WriteLine("confirm link: " + confirmLink);

            var emailBody = $"""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Confirm Your Email</title>
                </head>
                <body style="margin: 0; padding: 0; background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;">
                    <table role="presentation" border="0" cellpadding="0" cellspacing="0" width="100%" style="background-color: #f4f7f6; padding: 40px 0;">
                        <tr>
                            <td align="center">
                                <table role="presentation" border="0" cellpadding="0" cellspacing="0" width="600" style="background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05);">
                        
                                    <!-- Header -->
                                    <tr>
                                        <td align="center" style="background: linear-gradient(135deg, #4f46e5 0%, #3b82f6 100%); padding: 40px 20px;">
                                            <h1 style="color: #ffffff; margin: 0; font-size: 26px; font-weight: 700; letter-spacing: 0.5px;">Welcome Aboard!</h1>
                                        </td>
                                    </tr>

                                    <!-- Body Content -->
                                    <tr>
                                        <td style="padding: 40px 30px; text-align: left;">
                                            <h2 style="color: #1f2937; font-size: 20px; margin-top: 0; margin-bottom: 20px;">Hello,</h2>
                                            <p style="color: #4b5563; font-size: 16px; line-height: 1.6; margin-bottom: 30px;">
                                                Thank you for signing up with us. We are thrilled to have you! To activate your account and start exploring our platform, please click the button below to confirm your email address:
                                            </p>

                                            <!-- Button Action -->
                                            <table role="presentation" border="0" cellpadding="0" cellspacing="0" width="100%">
                                                <tr>
                                                    <td align="center" style="padding-bottom: 30px;">
                                                        <a href="{confirmLink}" target="_blank" style="background-color: #4f46e5; color: #ffffff; padding: 14px 32px; border-radius: 8px; text-decoration: none; font-size: 16px; font-weight: 600; display: inline-block; box-shadow: 0 4px 10px rgba(79, 70, 229, 0.3);">Confirm Email</a>
                                                    </td>
                                                </tr>
                                            </table>

                                            <p style="color: #6b7280; font-size: 14px; line-height: 1.5; margin-bottom: 20px;">
                                                If you did not create an account, you can safely ignore this email.
                                            </p>
                                
                                            <hr style="border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;">

                                            <p style="color: #9ca3af; font-size: 12px; line-height: 1.4; margin: 0;">
                                                If you're having trouble clicking the button, copy and paste the URL below into your web browser:<br>
                                                <a href="{confirmLink}" style="color: #4f46e5; word-break: break-all;">{confirmLink}</a>
                                            </p>
                                        </td>
                                    </tr>

                                    <!-- Footer -->
                                    <tr>
                                        <td align="center" style="background-color: #f9fafb; padding: 20px; text-align: center;">
                                            <p style="color: #9ca3af; font-size: 13px; margin: 0;">&copy; 2026 All rights reserved.</p>
                                        </td>
                                    </tr>

                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>
                """;

            await emailService.SendEmailAsync(
                user.Email,
                "Confirm your email",
                emailBody
            );

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return FluentValidationExtension.FromException(details: ex.Message);
        }
    }
    private async Task<Result> SendResetPasswordEmailScoped(
    ApplicationUser user,
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IConfiguration configuration)
    {
        try
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var resetPasswordLink = $"{configuration["FrontendAPIs:ResetPasswordFrontend"]}?email={user.Email}&token={encodedToken}";

            var emailBody = $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Reset Your Password</title>
            </head>
            <body style="margin: 0; padding: 0; background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;">
                <table role="presentation" border="0" cellpadding="0" cellspacing="0" width="100%" style="background-color: #f4f7f6; padding: 40px 0;">
                    <tr>
                        <td align="center">
                            <table role="presentation" border="0" cellpadding="0" cellspacing="0" width="600" style="background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05);">
                                
                                <!-- Header with Security/Warning Color Accent -->
                                <tr>
                                    <td align="center" style="background: linear-gradient(135deg, #f59e0b 0%, #ef4444 100%); padding: 40px 20px;">
                                        <h1 style="color: #ffffff; margin: 0; font-size: 26px; font-weight: 700; letter-spacing: 0.5px;">Password Reset Request</h1>
                                    </td>
                                </tr>

                                <!-- Body Content -->
                                <tr>
                                    <td style="padding: 40px 30px; text-align: left;">
                                        <h2 style="color: #1f2937; font-size: 20px; margin-top: 0; margin-bottom: 20px;">Hello,</h2>
                                        <p style="color: #4b5563; font-size: 16px; line-height: 1.6; margin-bottom: 30px;">
                                            We received a request to reset your password. If you made this request, please click the button below to choose a new password:
                                        </p>

                                        <!-- Button Action -->
                                        <table role="presentation" border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td align="center" style="padding-bottom: 30px;">
                                                    <a href="{resetPasswordLink}" target="_blank" style="background-color: #ef4444; color: #ffffff; padding: 14px 32px; border-radius: 8px; text-decoration: none; font-size: 16px; font-weight: 600; display: inline-block; box-shadow: 0 4px 10px rgba(239, 68, 68, 0.3);">Reset Password</a>
                                                </td>
                                            </tr>
                                        </table>

                                        <p style="color: #6b7280; font-size: 14px; line-height: 1.5; margin-bottom: 20px;">
                                            If you did not request a password reset, please ignore this email. Your password will remain unchanged.
                                        </p>
                                        
                                        <hr style="border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;">

                                        <p style="color: #9ca3af; font-size: 12px; line-height: 1.4; margin: 0;">
                                            If you're having trouble clicking the button, copy and paste the URL below into your web browser:<br>
                                            <a href="{resetPasswordLink}" style="color: #ef4444; word-break: break-all;">{resetPasswordLink}</a>
                                        </p>
                                    </td>
                                </tr>

                                <!-- Footer -->
                                <tr>
                                    <td align="center" style="background-color: #f9fafb; padding: 20px; text-align: center;">
                                        <p style="color: #9ca3af; font-size: 13px; margin: 0;">&copy; 2026 All rights reserved.</p>
                                    </td>
                                </tr>

                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;

            await emailService.SendEmailAsync(
                user.Email,
                "Reset Your Password",
                emailBody
            );

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return FluentValidationExtension.FromException(details: ex.Message);
        }
    }

    public async Task<Result<AuthResponseDTO>> RefreshTokenAsync(CancellationToken cancellationToken, RefreshTokenRequestDTO requestDTO)
    {
        var refreshToken = await _refreshTokenRepo.GetByRefreshToken(requestDTO.RefreshToken, cancellationToken);

        if (refreshToken is null)
            return Result.Fail(new UnauthorizedError("Invalid token."));

        if (refreshToken.IsRevoked || refreshToken.IsExpired)
            return Result.Fail(new UnauthorizedError("Expired token."));

        // ROTATE TOKEN (Best Practice)
        refreshToken.IsRevoked = true;

        var newRefreshToken = _jWTTokenService.GenerateRefreshToken();


        var refreshEntity = new UserRefreshToken()
        {
            RefreshToken = refreshToken.RefreshToken,
            UserId = refreshToken.UserId,
            User = refreshToken.User,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(15),

        };


        await _refreshTokenRepo.AddToken(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        var newAccessToken = await _jWTTokenService.GenerateToken(refreshToken.User);

        var response = new AuthResponseDTO(
           newAccessToken,
           newRefreshToken,
           DateTime.UtcNow.AddDays(1)
       );
        return Result.Ok(response);
    }



    public async Task<Result<IdentifyEmailResponseDTO>> IdentifyEmailAsync(IdentifyEmailRequestDTO request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user != null && user.EmailConfirmed)
        {
            user.FullName = request.FullName ?? request.Email.Split('@')[0];
            user.PhoneNumber = !string.IsNullOrWhiteSpace(request.PhoneNumber) && !string.IsNullOrWhiteSpace(request.CountryIsoCode) ? FormatPhone(request.PhoneNumber, request.CountryIsoCode) : null;
            return Result.Ok(new IdentifyEmailResponseDTO("Confirmed", false, "Email already confirmed. Please proceed to login or payment."));
        }

        var code = new Random().Next(100000, 999999).ToString();
        var otp = new OTP(request.Email, code, DateTime.UtcNow.AddMinutes(10), OtpPurpose.CheckoutVerification);

        var otpRepo = _unitOfWork.GetRepository<OTP, Guid>();
        await otpRepo.AddAsync(otp);
        await _unitOfWork.SaveChangesAsync();

        var email = request.Email;

        await _backgroundTaskQueue.EnqueueAsync(async (serviceProvider, cancellationToken) =>
        {
            using var scope = serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            await SendOtpEmailScoped(email, code, emailService);
        });

        var status = user == null ? "NotFound" : "Unconfirmed";
        var message = user == null
            ? "Account not found. We will create one for you after verification."
            : "Account exists but email is not confirmed. Please verify your identity.";

        return Result.Ok(new IdentifyEmailResponseDTO(status, true, message));
    }

    public async Task<Result<LoginResponseDTO>> VerifyOTPCheckoutAsync(VerifyOTPCheckoutRequestDTO request)
    {
        var validationResult = await _validationService.ValidateAsync(request);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }
        var otpRepo = _unitOfWork.GetRepository<OTP, Guid>();
        var spec = new OTPVerifySpecification(request.Email, request.Code, OtpPurpose.CheckoutVerification);
        var isValid = await otpRepo.AnyAsync(spec);

        if (!isValid)
        {
            return Result.Fail<LoginResponseDTO>("Invalid or expired verification code.");
        }

        await otpRepo.RemoveWhereAsync(x => x.Email == request.Email && x.purpose == OtpPurpose.CheckoutVerification);
        await _unitOfWork.SaveChangesAsync();

        var user = await _userManager.FindByEmailAsync(request.Email);
        bool isNewAccount = false;

        if (user == null)
        {
            user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                FullName = request.FullName ?? request.Email.Split('@')[0],
                EmailConfirmed = true,
                IsActive = true,
                PhoneNumber = !string.IsNullOrWhiteSpace(request.PhoneNumber) && !string.IsNullOrWhiteSpace(request.CountryIsoCode) ? FormatPhone(request.PhoneNumber, request.CountryIsoCode) : null,

            };

            var tempPassword = "Amigo@" + Guid.NewGuid().ToString("N").Substring(0, 10);
            var createResult = await _userManager.CreateAsync(user, tempPassword);

            if (!createResult.Succeeded)
            {
                return FluentValidationExtension.FromIdentityErrors(createResult.Errors);
            }

            await _userManager.AddToRoleAsync(user, "Public");
            isNewAccount = true;

            var userId = user.Id;
            var fullName = user.FullName;

            await _backgroundTaskQueue.EnqueueAsync(async (serviceProvider, cancellationToken) =>
            {
                using var scope = serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                var backgroundUser = await userManager.FindByIdAsync(userId.ToString());
                if (backgroundUser != null)
                {
                    await SendAccountCreatedEmailScoped(backgroundUser, userManager, emailService, configuration);
                }
            });
        }
        else
        {
            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }
        }

        string role = await GetRole(user);
        var data = new LoginResponseDTO
        (
            FullName: user.FullName ?? user.UserName,
            Email: user.Email,
            AccessToken: await _jWTTokenService.GenerateToken(user),
            RefreshToken: _jWTTokenService.GenerateRefreshToken(),
            AccessTokenExpiresIn: DateTime.UtcNow.AddDays(1),
            Role: role,
            EmailConfirmed: user.EmailConfirmed
        );

        var refreshToken = new UserRefreshToken()
        {
            RefreshToken = data.RefreshToken,
            UserId = user.Id,
            User = user,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(15),
        };
        await _refreshTokenRepo.AddToken(refreshToken, default);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(data).WithSuccess(new Success(isNewAccount ? "Account created and verified successfully!" : "Identity verified successfully!"));
    }
    private string FormatPhone(string phone, string region)
    {
        var number = _phoneUtil.Parse(phone, region);
        return _phoneUtil.Format(number, PhoneNumberFormat.E164);
    }

    private async Task SendOtpEmailScoped(string email, string code, IEmailService emailService)
    {
        var emailBody = $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Verification Code</title>
            </head>
            <body style="margin: 0; padding: 0; background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;">
                <table role="presentation" border="0" cellpadding="0" cellspacing="0" width="100%" style="background-color: #f4f7f6; padding: 40px 0;">
                    <tr>
                        <td align="center">
                            <table role="presentation" border="0" cellpadding="0" cellspacing="0" width="600" style="background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05);">
                                
                                <!-- Header -->
                                <tr>
                                    <td align="center" style="background: linear-gradient(135deg, #db2777 0%, #f43f5e 100%); padding: 35px 20px;">
                                        <h1 style="color: #ffffff; margin: 0; font-size: 24px; font-weight: 700; letter-spacing: 0.5px;">Amigo Arabe Tours</h1>
                                    </td>
                                </tr>

                                <!-- Body Content -->
                                <tr>
                                    <td style="padding: 40px 30px; text-align: left;">
                                        <h2 style="color: #1f2937; font-size: 20px; margin-top: 0; margin-bottom: 15px;">Verification Code</h2>
                                        <p style="color: #4b5563; font-size: 16px; line-height: 1.6; margin-bottom: 25px;">
                                            Hello, <br>You requested a verification code for your checkout process at Amigo Tourism. Please use the code below:
                                        </p>

                                        <!-- OTP Box -->
                                        <table role="presentation" border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td align="center" style="padding-bottom: 25px;">
                                                    <div style="background-color: #fdf2f8; border: 2px dashed #db2777; padding: 15px 30px; border-radius: 10px; display: inline-block;">
                                                        <span style="font-size: 32px; font-weight: bold; letter-spacing: 8px; color: #db2777;">{code}</span>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>

                                        <p style="color: #6b7280; font-size: 14px; line-height: 1.5; margin-bottom: 20px;">
                                            This code will expire in <strong>10 minutes</strong>. If you didn't request this, please safely ignore this email.
                                        </p>
                                    </td>
                                </tr>

                                <!-- Footer -->
                                <tr>
                                    <td align="center" style="background-color: #f9fafb; padding: 20px; text-align: center;">
                                        <p style="color: #9ca3af; font-size: 13px; margin: 0;">&copy; 2026 Amigo Arabe Tours. All rights reserved.</p>
                                    </td>
                                </tr>

                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;

        await emailService.SendEmailAsync(
            email,
            "Verification Code for Amigo Arabe Tours Checkout",
            emailBody
        );
    }

    private async Task SendAccountCreatedEmailScoped(
        ApplicationUser user,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IConfiguration configuration)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebUtility.UrlEncode(token);
        var resetPasswordLink = $"{configuration["FrontendAPIs:ResetPasswordFrontend"]}?email={user.Email}&token={encodedToken}";

        var emailBody = $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Welcome to Amigo Arabe Tours</title>
            </head>
            <body style="margin: 0; padding: 0; background-color: #f4f7f6; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;">
                <table role="presentation" border="0" cellpadding="0" cellspacing="0" width="100%" style="background-color: #f4f7f6; padding: 40px 0;">
                    <tr>
                        <td align="center">
                            <table role="presentation" border="0" cellpadding="0" cellspacing="0" width="600" style="background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05);">
                                
                                <!-- Header -->
                                <tr>
                                    <td align="center" style="background: linear-gradient(135deg, #db2777 0%, #f43f5e 100%); padding: 40px 20px;">
                                        <h1 style="color: #ffffff; margin: 0; font-size: 26px; font-weight: 700; letter-spacing: 0.5px;">Welcome to Amigo Tours!</h1>
                                    </td>
                                </tr>

                                <!-- Body Content -->
                                <tr>
                                    <td style="padding: 40px 30px; text-align: left;">
                                        <h2 style="color: #1f2937; font-size: 20px; margin-top: 0; margin-bottom: 20px;">Hello {user.FullName},</h2>
                                        <p style="color: #4b5563; font-size: 16px; line-height: 1.6; margin-bottom: 25px;">
                                            We have successfully created an account for you to manage your bookings and track your tours seamlessly.
                                        </p>
                                        <p style="color: #4b5563; font-size: 16px; line-height: 1.6; margin-bottom: 30px;">
                                            To secure your account and access your dashboard, please click the button below to set your password:
                                        </p>

                                        <!-- Button Action -->
                                        <table role="presentation" border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td align="center" style="padding-bottom: 30px;">
                                                    <a href="{resetPasswordLink}" target="_blank" style="background-color: #db2777; color: #ffffff; padding: 14px 32px; border-radius: 8px; text-decoration: none; font-size: 16px; font-weight: 600; display: inline-block; box-shadow: 0 4px 10px rgba(219, 39, 119, 0.3);">Set Your Password</a>
                                                </td>
                                            </tr>
                                        </table>

                                        <p style="color: #6b7280; font-size: 14px; line-height: 1.5; margin-bottom: 20px;">
                                            After setting your password, you will be able to log in and view all your tour vouchers anytime.
                                        </p>
                                        
                                        <hr style="border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;">

                                        <p style="color: #9ca3af; font-size: 12px; line-height: 1.4; margin: 0;">
                                            If you're having trouble clicking the button, copy and paste the URL below into your web browser:<br>
                                            <a href="{resetPasswordLink}" style="color: #db2777; word-break: break-all;">{resetPasswordLink}</a>
                                        </p>
                                    </td>
                                </tr>

                                <!-- Footer -->
                                <tr>
                                    <td align="center" style="background-color: #f9fafb; padding: 20px; text-align: center;">
                                        <p style="color: #9ca3af; font-size: 13px; margin: 0;">&copy; 2026 Amigo Arabe Tours. All rights reserved.</p>
                                    </td>
                                </tr>

                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;

        await emailService.SendEmailAsync(
            user.Email,
            "Account Created Successfully - Amigo Arabe Tours",
            emailBody
        );
    }
}



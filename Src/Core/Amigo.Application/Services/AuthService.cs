

using Amigo.Application.BackgroundTasks;
using Amigo.Application.Specifications.Identity;
using Amigo.Domain.DTO.Authentication;
using Amigo.Domain.Entities.Identity;
using Amigo.Domain.Enum;
using Microsoft.AspNetCore.Hosting;
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
                         IJWTTokenService _jWTTokenService,
                IWebHostEnvironment env
    ) : IAuthService
{
    private string? _Template;
    private string? _cachedReminderTemplate;

    private readonly IWebHostEnvironment _env = env;


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
            var html = BuildHtml("confirm-email.html");


            await emailService.SendEmailAsync(
                user.Email,
                "Confirm your email",
                html
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

            var emailBody = BuildHtml("reset-password.html");
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
        var emailBody = BuildHtml("send-otp.html");

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

        var emailBody = BuildHtml("create-account.html");

        await emailService.SendEmailAsync(
            user.Email,
            "Account Created Successfully - Amigo Arabe Tours",
            emailBody
        );
    }
    private string BuildHtml(string fileName)
    {
        var template = LoadTemplate(fileName);

        return template
            
            .Replace("{{WebsiteLink}}", _configuration["ContactInfo:WebsiteLink"])
            .Replace("{{FacebookLink}}", _configuration["ContactInfo:FacebookLink"])
            .Replace("{{YoutubeLink}}", _configuration["ContactInfo:YoutubeLink"])
            .Replace("{{InstaLink}}", _configuration["ContactInfo:InstaLink"])
            .Replace("{{CreatedAt}}", _configuration["ContactInfo:CreatedAt"]);

    }

    private string LoadTemplate(string fileName)
    {
        if (_Template != null)
            return _Template;

        var path = Path.Combine(_env.ContentRootPath, "Templates", fileName);
        _Template = File.ReadAllText(path);

        return _Template;
    }
}



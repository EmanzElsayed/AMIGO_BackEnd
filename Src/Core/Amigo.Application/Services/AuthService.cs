

using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Amigo.Domain.Entities.Identity;
using Amigo.Domain.Enum;
using Amigo.Application.Specifications.Identity;
using Amigo.Domain.DTO.Authentication;
using System.Net;

namespace Amigo.Application.Services;

public class AuthService(
                         UserManager<ApplicationUser> _userManager,
                         IRefreshTokenRepo _refreshTokenRepo,

                        IValidationService _validationService,
                         IConfiguration _configuration,
                         IEmailService _emailService,
                         IUnitOfWork _unitOfWork,
                         IJWTTokenService _jWTTokenService
    ) : IAuthService
{
    

    public async Task<Result<LoginResponseDTO>> LoginAsync(LoginRequestDTO requestDTO  , CancellationToken cancellationToken)
    {
        var validationResult = await _validationService.ValidateAsync(requestDTO);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var user = await _userManager.FindByEmailAsync(requestDTO.Email);

        if (user is null)   
        {
            return Result.Fail(new UnauthorizedError());
                                

        }

        //if (!user.EmailConfirmed)
        //{
        //    var SendEmailResult = await SendConfirmEmail(user, requestDTO.ReturnUrl);
        //    if (!SendEmailResult.IsSuccess)
        //    {
        //        return SendEmailResult;
        //    }
        //    return Result.Fail(new ForbiddenError("Please Confirm Your Email First"));


        //}
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
                FullName : user.FullName?? user.UserName,
                Email: requestDTO.Email,
                AccessToken: await _jWTTokenService.GenerateToken(user) ,
                RefreshToken : _jWTTokenService.GenerateRefreshToken(),
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
            await _refreshTokenRepo.AddToken(refreshToken , cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Ok(data)
                 .WithSuccess(new Success("Welcome To Amigo Arabe Tours"));
                 

        }
        else
        {
            return Result.Fail(new UnauthorizedError());
                                  

        }
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

        if (!IsEmailExist(user))
        {
            return Result.Fail(new NotFoundEmailError(requestDTO.Email));


        }
        var resetEmailResult =  await SendResetPasswordEmail(user);
        if(!resetEmailResult.IsSuccess)
        {
            return resetEmailResult;
        }

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
        if ( IsEmailExist(existingUser))
        {
            return Result.Fail(new EmailAlreadyExistsError(requestDTO.Email));


        }

        #endregion

        #region Check Exist Email And It is not Confirmed And Resing Email

        var ExistEmailAndNotConfirmedResponse =  await IsEmailExistAndNotConfirmedAndResingEmail(existingUser, requestDTO.ReturnUrl);
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
        await _userManager.AddToRoleAsync(user, "User");

       
       

        var SendEmailResult =   await SendConfirmEmail(user, requestDTO.ReturnUrl);
        if (!SendEmailResult.IsSuccess)
        { 
            return SendEmailResult;
        }

      

        return Result.Ok()
            .WithSuccess(new Success("Registration successful. Please confirm your email using the link sent to your inbox") 
            .WithMetadata("StatusCode",(int) HttpStatusCode.Created ));



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
            var SendEmailResult = await SendConfirmEmail(user, requestDTO.ReturnUrl);
            if (!SendEmailResult.IsSuccess)
            {
                return SendEmailResult;
            }
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
            await SendConfirmEmail(existingUser, returnUrl);

          

            return Result.Ok()
                .WithSuccess(new Success("Email already exists but not confirmed. Please check your email.")
            );

        }
        else return null;

    }
    private async Task<Result> SendConfirmEmail(ApplicationUser user, string? returnUrl = null)
    {
        try
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var confirmLink = $"{_configuration["FrontendAPIs:ConfirmEmailFrontend"]}?email={user.Email}&token={encodedToken}";
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

        var response =  new AuthResponseDTO(
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
            return Result.Ok(new IdentifyEmailResponseDTO("Confirmed", false, "Email already confirmed. Please proceed to login or payment."));
        }

        var code = new Random().Next(100000, 999999).ToString();
        var otp = new OTP(request.Email, code, DateTime.UtcNow.AddMinutes(10), OtpPurpose.CheckoutVerification);

        var otpRepo = _unitOfWork.GetRepository<OTP, Guid>();
        await otpRepo.AddAsync(otp);
        await _unitOfWork.SaveChangesAsync();

        await _emailService.SendEmailAsync(
            request.Email,
            "Verification Code for Amigo Checkout",
            $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
                    <h2 style='color: #db2777;'>Verification Code</h2>
                    <p>Hello,</p>
                    <p>You requested a verification code for your checkout process at Amigo Arabe Tours.</p>
                    <div style='background: #fdf2f8; padding: 15px; border-radius: 8px; text-align: center; margin: 20px 0;'>
                        <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #db2777;'>{code}</span>
                    </div>
                    <p>This code will expire in 10 minutes.</p>
                    <p>If you didn't request this, please ignore this email.</p>
                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='font-size: 12px; color: #999;'>Amigo Arabe Tours Team</p>
                </div>"
        );

        var status = user == null ? "NotFound" : "Unconfirmed";
        var message = user == null 
            ? "Account not found. We will create one for you after verification." 
            : "Account exists but email is not confirmed. Please verify your identity.";

        return Result.Ok(new IdentifyEmailResponseDTO(status, true, message));
    }

    public async Task<Result<LoginResponseDTO>> VerifyOTPCheckoutAsync(VerifyOTPCheckoutRequestDTO request)
    {
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
                IsActive = true
            };

            var tempPassword = "Amigo@" + Guid.NewGuid().ToString("N").Substring(0, 10);
            var createResult = await _userManager.CreateAsync(user, tempPassword);
            
            if (!createResult.Succeeded)
            {
                return FluentValidationExtension.FromIdentityErrors(createResult.Errors);
            }

            await _userManager.AddToRoleAsync(user, "User");
            isNewAccount = true;

            
            await SendAccountCreatedEmail(user);
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

    private async Task SendAccountCreatedEmail(ApplicationUser user)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebUtility.UrlEncode(token);
        var resetPasswordLink = $"{_configuration["FrontendAPIs:ResetPasswordFrontend"]}?email={user.Email}&token={encodedToken}";

        await _emailService.SendEmailAsync(
            user.Email,
            "Account Created Successfully - Amigo Arabe Tours",
            $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
                    <h2 style='color: #db2777;'>Welcome to Amigo Arabe Tours!</h2>
                    <p>Hello <b>{user.FullName}</b>,</p>
                    <p>We have created an account for you to manage your bookings easily.</p>
                    <p>To secure your account, please click the link below to set your password:</p>
                    <div style='margin: 30px 0;'>
                        <a href='{resetPasswordLink}' style='background: #db2777; color: white; padding: 12px 25px; text-decoration: none; border-radius: 50px; font-weight: bold;'>Set Your Password</a>
                    </div>
                    <p>After setting your password, you can log in and view all your tour vouchers in your dashboard.</p>
                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='font-size: 12px; color: #999;'>Thank you for choosing Amigo Arabe Tours!</p>
                </div>"
        );
    }
}



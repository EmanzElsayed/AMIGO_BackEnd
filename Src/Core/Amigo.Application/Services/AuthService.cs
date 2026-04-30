

using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Amigo.Application.Services;

public class AuthService(
                         UserManager<ApplicationUser> _userManager,
                         IRefreshTokenRepo _refreshTokenRepo,

                        IValidationService _validationService,
                         IConfiguration _configuration,
                         IEmailService _emailService,
                         IUserMapping _userMapping,IUnitOfWork _unitOfWork,
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

        var user = _userMapping.ToEntity(requestDTO);

        var createResult = await _userManager.CreateAsync(user, requestDTO.Password);

        if (!createResult.Succeeded)
        {
            return FluentValidationExtension.FromIdentityErrors(createResult.Errors);
        }
        await _userManager.AddToRoleAsync(user, "Customer");

       
       

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
        var primaryRole = roles.FirstOrDefault() ?? "Customer";
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


}


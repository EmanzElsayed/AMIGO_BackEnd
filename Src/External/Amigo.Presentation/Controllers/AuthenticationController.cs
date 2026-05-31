using Amigo.Application.Abstraction.Services;
using Amigo.Infrastructure;
using Microsoft.Extensions.Localization;
using PayPalCheckoutSdk.Core;

namespace Amigo.Presentation.Controllers
{
    
    [Route("api/v1/auth")]
    public class AuthenticationController( IServiceManager _serviceManager)
        : BaseController
    {

        [HttpPost("register")]
        public async Task<IResultBase> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {

            return await _serviceManager.AuthService.RegisterAsync(registerRequestDTO);
            
           
        }
        [HttpPost("login")]
        public async Task<IResultBase> Login(CancellationToken cancellationToken,[FromBody] LoginRequestDTO loginRequestDTO)
        {
           
            return await _serviceManager.AuthService.LoginAsync(loginRequestDTO, cancellationToken);

          

        }

        [HttpPost("confirm-email")]
        public async Task<IResultBase> ConfirmEmail([FromBody] ConfirmEmailRequestDTO confirmEmailDTO)
        {
            return await _serviceManager.AuthService.ConfirmEmail(confirmEmailDTO);

            
        }
       
        [HttpPost("resend-confirmation")]
        public async Task<IResultBase> ResendConfirmEmail([FromBody] ResendConfrimEmailRequestDTO requestDTO)
        {
            return await _serviceManager.AuthService.ResendConfirmEmail(requestDTO);

        }

        [HttpPost("forget-password")]
        public async Task<IResultBase> ForgotPassword([FromBody] ForgetPasswordRequestDTO requestDTO)
        {
            return await _serviceManager.AuthService.ForgetPassword(requestDTO);

           
        }


        [HttpPost("reset-password")]
        public async Task<IResultBase> ResetPasswod([FromBody] ResetPasswordRequestDTO requestDTO)
        {
            return await _serviceManager.AuthService.ResetPassword(requestDTO);

        }
        [HttpPost("refresh")]
        public async Task<IResultBase> RefreshToken(CancellationToken cancellationToken, [FromBody] RefreshTokenRequestDTO requestDTO)
        {
            return await _serviceManager.AuthService.RefreshTokenAsync(cancellationToken, requestDTO);

        }

        [HttpPost("identify-checkout")]
        public async Task<IResultBase> IdentifyEmail([FromBody] IdentifyEmailRequestDTO requestDTO)
        {
            return await _serviceManager.AuthService.IdentifyEmailAsync(requestDTO);
        }

        [HttpPost("verify-checkout-otp")]
        public async Task<IResultBase> VerifyOTPCheckout([FromBody] VerifyOTPCheckoutRequestDTO requestDTO)
        {
            return await _serviceManager.AuthService.VerifyOTPCheckoutAsync(requestDTO);
        }
    }
}
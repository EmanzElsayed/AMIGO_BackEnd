

using PayPalCheckoutSdk.Core;

namespace Amigo.Presentation.Controllers
{
    
    [Route("api/v1/auth")]
    public class AuthenticationController(IAuthService _authService) 
        : BaseController
    {
        [HttpPost("register")]
        public async Task<IResultBase> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            return await _authService.RegisterAsync(registerRequestDTO);
            
           
        }
        [HttpPost("login")]
        public async Task<IResultBase> Login(CancellationToken cancellationToken,[FromBody] LoginRequestDTO loginRequestDTO)
        {
            return await _authService.LoginAsync(loginRequestDTO, cancellationToken);

          

        }

        [HttpPost("confirm-email")]
        public async Task<IResultBase> ConfirmEmail([FromBody] ConfirmEmailRequestDTO confirmEmailDTO)
        {
            return await _authService.ConfirmEmail(confirmEmailDTO);

            
        }
       
        [HttpPost("resend-confirmation")]
        public async Task<IResultBase> ResendConfirmEmail([FromBody] ResendConfrimEmailRequestDTO requestDTO)
        {
            return await _authService.ResendConfirmEmail(requestDTO);

        }

        [HttpPost("forget-password")]
        public async Task<IResultBase> ForgotPassword([FromBody] ForgetPasswordRequestDTO requestDTO)
        {
            return await _authService.ForgetPassword(requestDTO);

           
        }


        [HttpPost("reset-password")]
        public async Task<IResultBase> ResetPasswod([FromBody] ResetPasswordRequestDTO requestDTO)
        {
            return await _authService.ResetPassword(requestDTO);

        }
        [HttpPost("refresh")]
        public async Task<IResultBase> RefreshToken(CancellationToken cancellationToken, [FromBody] RefreshTokenRequestDTO requestDTO)
        {
            return await _authService.RefreshTokenAsync(cancellationToken, requestDTO);

        }

        [HttpPost("identify-checkout")]
        public async Task<IResultBase> IdentifyEmail([FromBody] IdentifyEmailRequestDTO requestDTO)
        {
            return await _authService.IdentifyEmailAsync(requestDTO);
        }

        [HttpPost("verify-checkout-otp")]
        public async Task<IResultBase> VerifyOTPCheckout([FromBody] VerifyOTPCheckoutRequestDTO requestDTO)
        {
            return await _authService.VerifyOTPCheckoutAsync(requestDTO);
        }
    }
}


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
        public async Task<IResultBase> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            return await _authService.LoginAsync(loginRequestDTO);

          

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

    }
}
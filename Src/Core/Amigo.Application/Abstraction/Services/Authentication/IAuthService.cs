
namespace Amigo.Application.Abstraction.Services.Authentication;
public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterRequestDTO request);
    Task<Result<LoginResponseDTO>> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken);


    Task<Result> ConfirmEmail(ConfirmEmailRequestDTO request);

    Task<Result> ResendConfirmEmail(ResendConfrimEmailRequestDTO requestDTO);

    Task<Result> ForgetPassword(ForgetPasswordRequestDTO requestDTO);
    Task<Result> ResetPassword(ResetPasswordRequestDTO requestDTO);



    Task<Result<AuthResponseDTO>> RefreshTokenAsync(CancellationToken cancellationToken, RefreshTokenRequestDTO requestDTO);
    Task<Result<IdentifyEmailResponseDTO>> IdentifyEmailAsync(IdentifyEmailRequestDTO request);
    Task<Result<LoginResponseDTO>> VerifyOTPCheckoutAsync(VerifyOTPCheckoutRequestDTO request);
}

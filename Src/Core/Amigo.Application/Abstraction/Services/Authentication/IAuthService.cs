
namespace Amigo.Application.Abstraction.Services.Authentication;
public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterRequestDTO request);
    Task<Result<LoginResponseDTO>> LoginAsync(LoginRequestDTO request);


    Task<Result> ConfirmEmail(ConfirmEmailRequestDTO request);

    Task<Result> ResendConfirmEmail(ResendConfrimEmailRequestDTO requestDTO);

    Task<Result> ForgetPassword(ForgetPasswordRequestDTO requestDTO);
    Task<Result> ResetPassword(ResetPasswordRequestDTO requestDTO);



    //Task<Result<LoginResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request);
}

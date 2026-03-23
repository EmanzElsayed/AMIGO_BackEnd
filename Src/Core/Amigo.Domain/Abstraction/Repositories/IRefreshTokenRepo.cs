
namespace Amigo.Domain.Abstraction.Repositories;

public interface IRefreshTokenRepo
{
    Task AddToken(UserRefreshToken refreshToken, CancellationToken cancellationToken);
    Task<UserRefreshToken> GetByRefreshToken(string refreshToken, CancellationToken cancellationToken);
    void UpdateAsync(UserRefreshToken refreshToken, CancellationToken cancellationToken);

}

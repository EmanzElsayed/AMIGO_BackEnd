using Amigo.Domain.Abstraction.Repositories;

namespace Amigo.Persistence.Repositories;

public class RefreshTokenRepo(AmigoDbContext dbContext) : IRefreshTokenRepo
{
    public async Task AddToken(UserRefreshToken refreshToken, CancellationToken cancellationToken)
    => await dbContext.AddAsync(refreshToken, cancellationToken);

    public async Task<UserRefreshToken?> GetByRefreshToken(string refreshToken, CancellationToken cancellationToken)
    => await dbContext.RefreshTokens.Include(x => x.User).FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);

    public void UpdateAsync(UserRefreshToken refreshToken, CancellationToken cancellationToken)
    => dbContext.Update(refreshToken);
}

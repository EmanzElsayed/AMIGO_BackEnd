using Amigo.Domain.Abstraction.Repositories;

namespace Amigo.Persistence.Repositories;

public class GenericRepo(AmigoDbContext dbContext) : IGenericRepo
{
    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

namespace Amigo.Domain.Abstraction.Repositories;

public interface IGenericRepo
{
    Task SaveChanges(CancellationToken cancellationToken);
}

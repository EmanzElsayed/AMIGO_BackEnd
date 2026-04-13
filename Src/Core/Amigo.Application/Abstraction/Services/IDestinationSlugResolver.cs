namespace Amigo.Application.Abstraction.Services;

public interface IDestinationSlugResolver
{
    Task<Guid?> ResolveDestinationIdAsync(string slug, CancellationToken cancellationToken = default);
}

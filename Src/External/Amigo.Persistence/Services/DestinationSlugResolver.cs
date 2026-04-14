using Amigo.Application.Abstraction.Services;
using Amigo.Application.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Amigo.Persistence.Services;

public class DestinationSlugResolver(AmigoDbContext _db) 
            : IDestinationSlugResolver
{
    public async Task<Guid?> ResolveDestinationIdAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return null;

        var pairs = await _db.Destinations
            .AsNoTracking()
            .Where(d => d.IsActive && !d.IsDeleted)
            .SelectMany(d => d.Translations.Select(t => new { d.Id, t.Name }))
            .ToListAsync(cancellationToken);

        return pairs.FirstOrDefault(p => SlugHelper.MatchesName(p.Name, slug))?.Id;
    }
}

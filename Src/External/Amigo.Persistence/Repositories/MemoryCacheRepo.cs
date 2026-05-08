using Amigo.Domain.Abstraction.Repositories;
using Microsoft.Extensions.Caching.Memory;

public class MemoryCacheRepo : ICacheRepo
{
    private readonly IMemoryCache _cache;

    public MemoryCacheRepo(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<string?> GetAsync(string cacheKey)
    {
        _cache.TryGetValue(cacheKey, out string? value);
        return Task.FromResult(value);
    }

    public Task SetAsync(string cacheKey, string value, TimeSpan timeToLive)
    {
        _cache.Set(cacheKey, value, new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5),
            AbsoluteExpirationRelativeToNow = timeToLive
        });

        return Task.CompletedTask;
    }
}
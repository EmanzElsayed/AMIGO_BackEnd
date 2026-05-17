using Amigo.Domain.Abstraction.Repositories;
using Microsoft.Extensions.Caching.Memory;

public class MemoryCacheRepo : ICacheRepo
{
    private readonly IMemoryCache _cache;

    public MemoryCacheRepo(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<T?> GetAsync<T>(string cacheKey)
    {
        _cache.TryGetValue(cacheKey, out T? value);

        return Task.FromResult(value);
    }

    public Task SetAsync<T>(
        string cacheKey,
        T value,
        TimeSpan timeToLive)
    {
        var options = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5),
            AbsoluteExpirationRelativeToNow = timeToLive
        };

        _cache.Set(cacheKey, value, options);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string cacheKey)
    {
        _cache.Remove(cacheKey);

        return Task.CompletedTask;
    }
}
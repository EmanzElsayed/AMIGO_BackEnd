public class CacheService : ICacheService
{
    private readonly ICacheRepo _cacheRepository;

    public CacheService(ICacheRepo cacheRepository)
    {
        _cacheRepository = cacheRepository;
    }

    public async Task<T?> GetAsync<T>(string cacheKey)
    {
        return await _cacheRepository.GetAsync<T>(cacheKey);
    }

    public async Task SetAsync<T>(
        string cacheKey,
        T value,
        TimeSpan timeToLive)
    {
        await _cacheRepository.SetAsync(
            cacheKey,
            value,
            timeToLive);
    }

    public async Task RemoveAsync(string cacheKey)
    {
        await _cacheRepository.RemoveAsync(cacheKey);
    }
}
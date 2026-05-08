using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Amigo.Application.Services
{
    public class CacheService(ICacheRepo _cacheRepository) : ICacheService
    {
        public async Task<string?> GetAsync(string cacheKey)
        {
            return await _cacheRepository.GetAsync(cacheKey);
        }

        public async Task SetAsync(string cacheKey, object value, TimeSpan timeToLive)
        {
            var cachedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await _cacheRepository.SetAsync(cacheKey, cachedValue, timeToLive);
        }
    }
}

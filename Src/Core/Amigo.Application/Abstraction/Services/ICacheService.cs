using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
            
        Task SetAsync<T>(
            string key,
            T value,
            TimeSpan ttl);

        Task RemoveAsync(string key);
    }
}

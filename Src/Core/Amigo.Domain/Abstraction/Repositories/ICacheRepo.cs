using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction.Repositories
{
    public interface ICacheRepo
    {
        Task<T?> GetAsync<T>(string cacheKey);

        Task SetAsync<T>( 
            string cacheKey,
            T value,
            TimeSpan timeToLive);

        Task RemoveAsync(string cacheKey);
    }
}

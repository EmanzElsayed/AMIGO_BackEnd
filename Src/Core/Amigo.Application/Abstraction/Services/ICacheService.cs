using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface ICacheService
    {
        Task<string?> GetAsync(string cacheKey);
        Task SetAsync(string cacheKey, object value, TimeSpan timeToLive);
    }
}

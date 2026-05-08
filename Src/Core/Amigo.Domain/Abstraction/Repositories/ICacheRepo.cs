using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction.Repositories
{
    public interface ICacheRepo
    {
        Task<string?> GetAsync(string cackeKey);
        Task SetAsync(string cackeKey, string value, TimeSpan timeToLive);
    }
}

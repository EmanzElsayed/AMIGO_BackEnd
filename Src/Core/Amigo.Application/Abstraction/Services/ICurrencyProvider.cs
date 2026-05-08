using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface ICurrencyProvider
    {
        Task<Result<Dictionary<CurrencyCode, decimal>>> GetRatesAsync(CurrencyCode baseCurrency);
    }
}

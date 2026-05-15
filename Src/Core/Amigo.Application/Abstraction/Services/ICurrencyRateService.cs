using Amigo.Domain.DTO.Currency;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface ICurrencyRateService
    {

         Task BulkUpsertAsync(
            List<CurrencyRateBulkItemDTO> rates);

        Task<Result<decimal>> ConvertAsync(
        decimal amount,
        CurrencyCode from,
        CurrencyCode to);

        Task SetRateAsync(
        CurrencyCode from,
        CurrencyCode to,
        decimal rate);
        Task<Result<decimal>> GetRateAsync(
           CurrencyCode from,
           CurrencyCode to,bool isHit);
    }
}

using Amigo.Application.Helpers;
using Amigo.Application.Specifications.CurrencyRateSpecification;
using Amigo.Domain.DTO.Currency;
using System.Globalization;


namespace Amigo.Application.Services
{
    public class CurrencyRateService(ICacheRepo _cacheRepo,
        IUnitOfWork _unitOfWork ) 
        : ICurrencyRateService
    {


        public async Task<Result<decimal>> GetRateAsync(
              CurrencyCode from,
              CurrencyCode to)
        {
            if (from == to)
                return 1m;

            // direct from base
            if (from == Constants.BaseCurrency)
            {
                return await GetDirectRateAsync(from, to);
            }

            // to base
            if (to == Constants.BaseCurrency)
            {
                var rate = await GetDirectRateAsync(to, from);
                if (!rate.IsSuccess)
                    return Result.Fail(rate.Errors);

                return Result.Ok(1 / rate.Value);
            }

            // cross currency
            var fromRate = await GetDirectRateAsync(
                Constants.BaseCurrency,
                from);

            var toRate = await GetDirectRateAsync(
                Constants.BaseCurrency,
                to);

            if (!fromRate.IsSuccess)
                return Result.Fail(fromRate.Errors);

            if (!toRate.IsSuccess)
                return Result.Fail(toRate.Errors);

            return Result.Ok(toRate.Value / fromRate.Value);
        }


        private async Task<Result<decimal>> GetDirectRateAsync(
            CurrencyCode from,
            CurrencyCode to)
        {
            var key = GetKey(from, to);

            // CACHE
               // 1. CACHE
            var cached = await _cacheRepo.GetAsync(key);
            if (!string.IsNullOrEmpty(cached) &&
                decimal.TryParse(
                cached,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var cachedRate))
            {
                return Result.Ok(cachedRate);
            }

            var repo = _unitOfWork.GetRepository<CurrencyRate, Guid>();

            // 2. DB fallback
            var dbRate = await repo.GetByIdAsync(new GetCurrencyRateWithCurrencyTypeSpecification(from,to));

            if (dbRate is null)
            {
                return Result.Fail(new NotFoundError($"Currency rate not found: {from} -> {to}"));

            }

            await CacheRate(from, to, dbRate.Rate);
            return Result.Ok(dbRate.Rate);
          
        }



        public async Task BulkUpsertAsync(
            List<CurrencyRateBulkItemDTO> rates)
        {
            var repo = _unitOfWork.GetRepository<CurrencyRate, Guid>();

            // 1. load existing rates once
            var targets = rates.Select(x => x.TargetCurrency).ToList();

            var existingRates = await repo.GetAllAsync(new GetAllCurrencyWithTargetSpecification(targets));

            var existingDict = existingRates
                .ToDictionary(x => x.TargetCurrency);

            var now = DateTime.UtcNow;

            var toAdd = new List<CurrencyRate>();

            foreach (var item in rates)
            {
                if (existingDict.TryGetValue(item.TargetCurrency, out var existing))
                {
                    existing.Rate = item.Rate;
                    existing.FetchedAt = now;
                    existing.ExpiresAt = now.AddHours(1);
                }
                else
                {
                    toAdd.Add(new CurrencyRate
                    {
                        BaseCurrency = item.BaseCurrency,
                        TargetCurrency = item.TargetCurrency,
                        Rate = item.Rate,
                        FetchedAt = now,
                        ExpiresAt = now.AddHours(1)
                    });
                }

                // cache update (fast, no DB)
                await CacheRate(item.BaseCurrency, item.TargetCurrency, item.Rate);
            }

            if (toAdd.Any())
                await repo.AddRangeAsync(toAdd);

            
            await _unitOfWork.SaveChangesAsync();
        }



        public async Task SetRateAsync(
         CurrencyCode from,
         CurrencyCode to,
         decimal rate)
        {

            // 1. Save DB
            var _repo = _unitOfWork.GetRepository<CurrencyRate, Guid>();
            var existing = await _repo.GetByIdAsync(new GetCurrencyRateWithCurrencyTypeSpecification(from, to)); 

            if (existing is null )
            {
                existing = new CurrencyRate
                {
                    BaseCurrency = from,
                    TargetCurrency = to,
                    Rate = rate,
                    FetchedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                };

                await _repo.AddAsync(existing);
            }
            else
            {
                existing.Rate = rate;
                existing.FetchedAt = DateTime.UtcNow;
                existing.ExpiresAt = DateTime.UtcNow.AddHours(1);
            }

            // 2. Cache
            await CacheRate(from, to, rate);
        }



        public async Task<Result< decimal>> ConvertAsync(
         decimal amount,
         CurrencyCode from,
         CurrencyCode to)
        {
            var rate = await GetRateAsync(from, to);
            if (rate.IsSuccess)
            { 
                return Result.Ok(amount * rate.Value);
            
            }
            return Result.Fail("Invalid Convertion");
        }

        private string GetKey(
          CurrencyCode from,
          CurrencyCode to)
        {
            return $"fx:{from}:{to}";
        }
        private async Task CacheRate(
          CurrencyCode from,
          CurrencyCode to,
          decimal rate)
        {
            var key = GetKey(from, to);

            await _cacheRepo.SetAsync(
                key,
                rate.ToString(CultureInfo.InvariantCulture),
                TimeSpan.FromHours(1));

        }
    }
}

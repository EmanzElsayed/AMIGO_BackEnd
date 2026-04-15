using Amigo.Application.Abstraction.Services;
using Amigo.Application.Services;
using Amigo.Domain.DTO.Authentication;
using Amigo.Domain.DTO.Currency;
using Amigo.Domain.Enum;
using Amigo.Presentation.Filters;
using Amigo.SharedKernal.DTOs.Authentication;
using Amigo.SharedKernal.QueryParams;
using FluentResults;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text.Json;


namespace Amigo.Presentation.Controllers
{
    
    [Route("api/v1/lookups")]
    public class EnumsController(
        IEnumService _enumService,
        ICurrencyService _currencyService,
        IHttpClientFactory _httpClientFactory,
        IConfiguration _configuration) : BaseController
    {
        [HttpGet("languages")]
        public IResultBase GetLanguages()
        {
            return _enumService.GetEnum<Language>();

         
        }

        [HttpGet("CurrencyCode")]
        public IResultBase GetCurrency()
        {
            return _enumService.GetEnum<CurrencyCode>();

            

        }

        [HttpGet("Currency")]
        public async Task<IResultBase> GetAllCurrency([FromQuery]  GetAllCurrencyQuery requestQuery)
        {
            return await _currencyService.GetAllCurrencyAsync(requestQuery);

        }
        [HttpGet("Currency/{id}")]
        public async Task<IResultBase> GetCurrencyById([FromQuery] GetAllCurrencyQuery requestQuery , string id)
        {
            return await _currencyService.GetCurrencyByIdAsync(id,requestQuery);

        }
        [HttpGet("Country")]
        public IResultBase GetCountry()
        {
            return _enumService.GetEnum<CountryCode>();



        }

        [HttpGet("currency-rates")]
        public async Task<IResultBase> GetCurrencyRates([FromQuery] string? @base = "USD")
        {
            var baseCurrency = string.IsNullOrWhiteSpace(@base) ? "USD" : @base.Trim().ToUpperInvariant();
            if (baseCurrency.Length != 3)
            {
                return Result.Fail("Base currency must be a 3-letter ISO code.");
            }

            var apiKey = _configuration["CurrencyApi:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return Result.Fail("CurrencyCode API key is not configured.");
            }

            var providerBaseUrl = _configuration["CurrencyApi:BaseUrl"] ?? "https://currencyapi.net/api/v2";
            var endpoint = QueryHelpers.AddQueryString(
                $"{providerBaseUrl.TrimEnd('/')}/rates",
                new Dictionary<string, string?>
                {
                    ["base"] = baseCurrency,
                    ["output"] = "json",
                    ["key"] = apiKey
                });

            using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            using var client = _httpClientFactory.CreateClient();
            using var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail($"CurrencyCode provider failed with status {(int)response.StatusCode}.");
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var payload = await JsonDocument.ParseAsync(stream);

            if (!payload.RootElement.TryGetProperty("rates", out var ratesElement) ||
                ratesElement.ValueKind != JsonValueKind.Object)
            {
                return Result.Fail("CurrencyCode provider returned an invalid rates payload.");
            }

            var rates = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in ratesElement.EnumerateObject())
            {
                var code = property.Name.Trim().ToUpperInvariant();
                if (code.Length != 3) continue;
                if (property.Value.ValueKind != JsonValueKind.Number) continue;
                if (!property.Value.TryGetDecimal(out var value)) continue;
                if (value <= 0) continue;
                rates[code] = value;
            }
            rates["USD"] = 1m;

            var result = Result.Ok(new
            {
                @base = baseCurrency,
                rates
            });
            return result;
        }
    }
}

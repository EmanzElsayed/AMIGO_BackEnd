using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.WebUtilities;

namespace Amigo.Application.Services;



public class CurrencyApiProvider : ICurrencyProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public CurrencyApiProvider(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<Result<Dictionary<CurrencyCode, decimal>>>
        GetRatesAsync(CurrencyCode baseCurrency)
    {
        var apiKey =
            _configuration["CurrencyApi:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
            return Result.Fail( new NotFoundError(
                "Currency API key is missing"));

        var baseUrl =
            _configuration["CurrencyApi:BaseUrl"]
            ?? "https://currencyapi.net/api/v2";

        var endpoint = QueryHelpers.AddQueryString(
            $"{baseUrl.TrimEnd('/')}/rates",
            new Dictionary<string, string?>
            {
                ["base"] = baseCurrency.ToString(),
                ["output"] = "JSON",
                ["key"] = apiKey
            });

        using var client =
            _httpClientFactory.CreateClient();

        using var response =
            await client.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            return Result.Fail(
                $"Currency API failed: {(int)response.StatusCode}");
        }

        await using var stream =
            await response.Content.ReadAsStreamAsync();

        using var json =
            await JsonDocument.ParseAsync(stream);

        if (!json.RootElement.TryGetProperty(
                "rates",
                out var ratesElement))
        {
          return  Result.Fail(
                "Invalid currency response");
        }

        var result =
            new Dictionary<CurrencyCode, decimal>();

        foreach (var property
            in ratesElement.EnumerateObject())
        {
            var code = property.Name
                .Trim()
                .ToUpperInvariant();

            if (!Enum.TryParse<CurrencyCode>(
                    code,
                    true,
                    out var currency))
            {
                continue;
            }

            if (property.Value.ValueKind !=
                JsonValueKind.Number)
            {
                continue;
            }

            if (!property.Value.TryGetDecimal(
                    out var rate))
            {
                continue;
            }

            if (rate <= 0)
                continue;

            result[currency] = rate;
        }

        // base currency always = 1
        result[baseCurrency] = 1m;

        return result;
    }
}

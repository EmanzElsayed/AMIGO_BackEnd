using Amigo.Application.Helpers;
using Amigo.Domain.DTO.Currency;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Amigo.Application.Services.BackGroundServices;
public  class CurrencyBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<CurrencyBackgroundService> logger)
    : BackgroundService
{
    private readonly TimeSpan _interval =
        TimeSpan.FromMinutes(30);

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Currency Background Service Started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncRates();
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Currency sync failed");
            }

            await Task.Delay(
                _interval,
                stoppingToken);
        }

        logger.LogInformation(
            "Currency Background Service Stopped");
    }

    private async Task SyncRates()
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var currencyService =
            scope.ServiceProvider
                .GetRequiredService<ICurrencyRateService>();

        var currencyProvider =
           scope.ServiceProvider
               .GetRequiredService<ICurrencyProvider>();

        var rates = await currencyProvider.GetRatesAsync(Constants.BaseCurrency);

        if (rates.IsSuccess)
        {
            var bulk = rates.Value
                .Where(x => x.Key != Constants.BaseCurrency)
                .Select(x => new CurrencyRateBulkItemDTO(
                    Constants.BaseCurrency,
                    x.Key,
                    x.Value))
                .ToList();

            await currencyService.BulkUpsertAsync(bulk);
        }

    }
        
}

   

using Amigo.Application.Specifications.AvailableSlotsSpecification;
using Amigo.Application.Specifications.GetBackGroundServicesSpecification;
using Amigo.Domain.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amigo.Application.Services;

/// <summary>
/// Production Background Worker
/// Runs periodic maintenance jobs:
/// 1. Delete pending reservations
/// 2. Expire unpaid orders
/// </summary>
public sealed class BookingBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<BookingBackgroundService> logger)
    : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Booking Background Service Started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunJobs(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Background job failed");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        logger.LogInformation("Booking Background Service Stopped");
    }

    private async Task RunJobs(CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await ExpireReservations(unitOfWork);
        await ExpireOrders(unitOfWork);
        await SlodOutSlots(unitOfWork);
    }

    // =====================================================
    // 1) Expire Pending Reservations
    // =====================================================
    private async Task ExpireReservations(IUnitOfWork _unitOfWork)
    {
        var now = DateTime.UtcNow;

        var repo = _unitOfWork.GetRepository<SlotReservation, Guid>();

        var reservations = await repo.GetAllAsync(
            new GetExpiredPendingReservationsSpecification(now));

        repo.RemoveRange(reservations);



        await _unitOfWork.SaveChangesAsync();
    }

    // =====================================================
    // 2) Expire Orders still unpaid
    // =====================================================
    private async Task ExpireOrders(IUnitOfWork _unitOfWork)
    {
        var now = DateTime.UtcNow;

        var repo = _unitOfWork.GetRepository<Order, Guid>();

        var orders = await repo.GetAllAsync(
            new GetPendingOrdersBeforeDateSpecification(now));

        foreach (var order in orders)
        {
            order.Status = OrderStatus.Failed;
        }

        await _unitOfWork.SaveChangesAsync();
    }
    // =====================================================
    // 3) SoldOutSlots 
    // =====================================================
    private async Task SlodOutSlots(IUnitOfWork _unitOfWork)
    {
        

        var repo = _unitOfWork.GetRepository<AvailableSlots, Guid>();

        var availableSlots = await repo.GetAllAsync(
            new GetAllAvailableSlots());

        if (!availableSlots.Any())
            return;

        var avialableSlotsIds = availableSlots.Select(availableSlots => availableSlots.Id).ToList();

        var reservationRepo = _unitOfWork.GetRepository<SlotReservation, Guid>();

        var reservationsCount = await reservationRepo.GetGroupedCountAsync(
            x => avialableSlotsIds.Contains(x.SlotId),
            x => x.SlotId
        );

        foreach (var slot in availableSlots)
        {
            var count = reservationsCount.ContainsKey(slot.Id)
                ? reservationsCount[slot.Id]
                : 0;
            slot.ReservedCount = count;
            if (count >= slot.MaxCapacity)
            {
                slot.AvailableTimeStatus = AvailableDateTimeStatus.SoldOut;
                slot.SetModifiedDate(DateTime.UtcNow);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }




}
using Amigo.Application.Specifications.GetBackGroundServicesSpecification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amigo.Application.Services;

/// <summary>
/// Production Background Worker
/// Runs periodic maintenance jobs:
/// 1. Expire pending reservations
/// 2. Expire unpaid orders
/// 3. Reconcile stuck payments
/// 4. Cleanup old data
/// </summary>
public sealed class BookingBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<BookingBackgroundService> logger)
    : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

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
        var orchestrator = scope.ServiceProvider.GetRequiredService<IPaymentOrchestrator>();

        await ExpireReservations(unitOfWork);
        await ExpireOrders(unitOfWork);
        //await ReconcilePendingPayments(unitOfWork, orchestrator);
        await CleanupOldReservations(unitOfWork);
    }

    // =====================================================
    // 1) Expire Pending Reservations
    // =====================================================
    private async Task ExpireReservations(IUnitOfWork uow)
    {
        var now = DateTime.UtcNow;

        var repo = uow.GetRepository<SlotReservation, Guid>();

        var reservations = await repo.GetAllAsync(
            new GetExpiredPendingReservationsSpecification(now));

        foreach (var reservation in reservations)
        {
            reservation.Status = ReservationStatus.Expired;

            var slot = reservation.Slot;

            if (slot is null)
                continue;

            slot.ReservedCount = Math.Max(0,
                slot.ReservedCount - reservation.Quantity);
        }

        await uow.SaveChangesAsync();
    }

    // =====================================================
    // 2) Expire Orders still unpaid
    // =====================================================
    private async Task ExpireOrders(IUnitOfWork uow)
    {
        var limit = DateTime.UtcNow.AddMinutes(-15);

        var repo = uow.GetRepository<Order, Guid>();

        var orders = await repo.GetAllAsync(
            new GetPendingOrdersBeforeDateSpecification(limit));

        foreach (var order in orders)
        {
            order.Status = OrderStatus.Expired;
        }

        await uow.SaveChangesAsync();
    }

    // =====================================================
    // 3) Reconcile Pending Payments
    // If webhook missed
    // =====================================================
    //private async Task ReconcilePendingPayments(
    //IUnitOfWork uow,
    //IPaymentOrchestrator orchestrator)
    //{
    //    var limit = DateTime.UtcNow.AddMinutes(-5);

    //    var repo = uow.GetRepository<Payment, Guid>();

    //    var payments = await repo.GetAllAsync(
    //        new GetOldPendingPaymentsSpecification(limit));

    //    foreach (var payment in payments)
    //    {
    //        try
    //        {
    //            await ProcessPayment(payment, orchestrator);
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.LogWarning(ex,
    //                "Payment reconciliation skipped for {PaymentId}",
    //                payment.Id);
    //        }
    //    }
    //}

    //private async Task ProcessPayment(
    //Payment payment,
    //IPaymentOrchestrator orchestrator)
    //{
    //    if (payment.Provider == PaymentProvider.Stripe)
    //    {
    //        var payload = CreateStripeFakeEvent(payment.PaymentProviderReferenceId);

    //        await orchestrator.HandleSuccessAsync(
    //            PaymentProvider.Stripe,
    //            payload);
    //    }
    //}
    // =====================================================
    // 4) Cleanup old expired reservations
    // =====================================================
    private async Task CleanupOldReservations(IUnitOfWork uow)
    {
        var oldDate = DateTime.UtcNow.AddDays(-30);

        var repo = uow.GetRepository<SlotReservation, Guid>();

        var oldReservations = await repo.GetAllAsync(
            new GetOldExpiredReservationsSpecification(oldDate));

        repo.RemoveRange(oldReservations);

        await uow.SaveChangesAsync();
    }
}
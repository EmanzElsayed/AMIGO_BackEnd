using Amigo.Application.Specifications.AvailableSlotsSpecification;
using Amigo.Application.Specifications.RefundSpecification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amigo.Application.Services.BackGroundServices
{
    public class RefundProcessorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RefundProcessorService> _logger;

        public RefundProcessorService(
            IServiceScopeFactory scopeFactory, ILogger<RefundProcessorService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessRefunds();

                await Task.Delay(
                    TimeSpan.FromSeconds(15),
                    stoppingToken);
            }
        }

        private async Task ProcessRefunds()
        {
            using var scope =
                _scopeFactory.CreateScope();

            var unitOfWork =
                scope.ServiceProvider
                    .GetRequiredService<IUnitOfWork>();

            var resolver =
                scope.ServiceProvider
                    .GetRequiredService<IRefundProviderResolver>();

            var outboxRepo =
                unitOfWork.GetRepository<OutboxMessage, Guid>();
            var refundRepo =
                       unitOfWork.GetRepository<Refund, Guid>();

            var slotRepo =
                          unitOfWork.GetRepository<
                              AvailableSlots,
                              Guid>();
            var now = DateTime.UtcNow;
            var messages =
                await outboxRepo.GetAllAsync(
                    new PendingRefundOutboxSpecification(now));

            foreach (var message in messages)
            {
                try
                {
                    var refundId =
                        Guid.Parse(message.Payload);

                   

                    var refund =
                        await refundRepo.GetByIdAsync(
                            new GetRefundByIdSpecification(refundId));

                    if (refund == null)
                        continue;

                    if (refund.Status ==
                        RefundStatus.Completed)
                    {
                        message.Status =
                            OutboxStatus.Completed;

                        continue;
                    }
                    if (refund.Status == RefundStatus.Processing)
                        continue;

                    refund.Status =
                        RefundStatus.Processing;


                    var payment =
                        refund.Payment;

                    var provider =
                        resolver.Resolve(
                            payment.Provider!.Value);

                    var result =
                        await provider.RefundAsync(
                            payment.ProviderCaptureId!,
                            refund.Amount,
                            payment.Currency,
                            refund.IdempotencyKey!);

                    if (result.Success)
                    {
                        refund.Status =
                            RefundStatus.Completed;

                        refund.ProviderRefundId =
                            result.RefundId;

                        refund.ProviderResponseJson =
                            result.RawResponse;

                        refund.RefundedAt =
                            DateTime.UtcNow;

                        // BOOKING

                        refund.Booking.Status =
                            BookingStatus.Cancelled;

                        // CANCELLATION REQUEST

                        refund.CancellationRequest.Status =
                            CancellationRequestStatus.Refunded;

                        refund.CancellationRequest.ProcessedAt =
                            DateTime.UtcNow;

                        // RELEASE SLOT

                      

                        var slot =
                            await slotRepo.GetByIdAsync(new GetSlotByIdIncludeReservationSpecification(refund.Booking.OrderItem.SlotId!.Value)
                               );

                        if (slot != null)
                        {
                            var qty =
                                refund.Booking.OrderItem
                                    .OrderedPrice
                                    .Sum(x => x.Quantity);

                            slot.ReservedCount -= qty;

                            if (slot.ReservedCount < 0)
                                slot.ReservedCount = 0;

                            var slotReservation =
                                slot.SlotReservations
                                    .FirstOrDefault(r =>
                                        r.OrderId ==
                                        refund.Booking.OrderItem.OrderId);

                            if (slotReservation != null)
                            {
                                slotReservation.Status =
                                    ReservationStatus.Cancelled;
                            }
                            else
                            {
                                _logger.LogWarning(
                                    "SlotReservation not found for RefundId: {RefundId}, OrderId: {OrderId}",
                                    refund.Id,
                                    refund.Booking.OrderItem.OrderId);

                                message.LastError =
                                    $"Warning: SlotReservation not found for order {refund.Booking.OrderItem.OrderId}";
                            }
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Available slot not found for RefundId: {RefundId}, SlotId: {SlotId}",
                                refund.Id,
                                refund.Booking.OrderItem.SlotId);

                            message.LastError =
                                $"Warning: Slot not found => {refund.Booking.OrderItem.SlotId}";
                        }

                        // OUTBOX
                        message.Status =
                            OutboxStatus.Completed;
                        message.ProcessedAtUtc =
                            DateTime.UtcNow;
                        await unitOfWork.SaveChangesAsync();

                    }
                    else
                    {
                        refund.Status =
                            RefundStatus.Failed;

                        refund.FailureReason =
                            result.FailureReason;

                        message.RetryCount++;

                        message.LastError =
                           result.FailureReason;

                        message.NextRetryAt =
                            DateTime.UtcNow.AddMinutes(5);

                        if (message.RetryCount >= 5)
                        {
                            message.Status =
                                OutboxStatus.Failed;
                        }
                    }

                    await unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    message.RetryCount++;

                    message.LastError = ex.ToString();

                    message.NextRetryAt =
                        DateTime.UtcNow.AddMinutes(5);

                    if (message.RetryCount >= 5)
                    {
                        message.Status =
                            OutboxStatus.Failed;
                    }

                    await unitOfWork.SaveChangesAsync();
                }
            }
        }
    }
}

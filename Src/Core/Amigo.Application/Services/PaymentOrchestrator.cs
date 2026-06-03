using Amigo.Application.Specifications.AvailableSlotsSpecification;
using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Application.Specifications.CartSpecification;
using Amigo.Application.Specifications.OrderSpecification;
using Amigo.Application.Specifications.PaymentSpecification;
using Amigo.Application.Specifications.RefundSpecification;
using Amigo.Application.Specifications.Travelers;
using Amigo.Application.Specifications.WebhookEventLogSpecification;
using Amigo.Domain.Abstraction;
using Amigo.Domain.Abstraction.Repositories;
using Amigo.Domain.DTO.Cart;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Amigo.Application.Services
{
    public class PaymentOrchestrator : IPaymentOrchestrator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentOrchestrator> _logger;
        public PaymentOrchestrator(IUnitOfWork unitOfWork, ILogger<PaymentOrchestrator> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        // SUCCESS FLOW


        public async Task HandleSuccessAsync(PaymentProvider provider, string payload)
        {
            var (providerRefId,eventId ,rawData ) = ExtractProviderData(provider, payload);
            //var providerRefId = "5WK96583WG697635C";
            //var eventId = "4455";
            //var rawData = "emo";
            var strategy = _unitOfWork.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _unitOfWork.BeginTransactionAsync();

                var eventRepo = _unitOfWork.GetRepository<WebhookEventLog, Guid>();
                var paymentRepo = _unitOfWork.GetRepository<Payment, Guid>();
                var outboxRepo = _unitOfWork.GetRepository<OutboxMessage, Guid>();

                // webhook event log 
                // check if webhook is sent before or not

                var exists = await eventRepo.AnyAsync(
               new GetWebhookWithEventIdSpecivfcation(eventId));

                if (exists)
                    return;


                // if not add new event log :
                await eventRepo.AddAsync(new WebhookEventLog
                {
                    Id = Guid.NewGuid(),
                    Provider = provider,
                    ProviderEventId = eventId,
                    Payload = payload,
                    Processed = false
                });

                //Update Payment

                var payment = await paymentRepo
                        .GetByIdAsync(new GetPaymentByProviderRefSpec(providerRefId));

                if (payment is null || payment.Status == PaymentStatus.Succeeded)
                    return;

                payment.Status = PaymentStatus.Succeeded;
                payment.PaidAt = DateTime.UtcNow;
                payment.RawResponseJson = rawData;


                // add Outbox Event
                
                await outboxRepo.AddAsync(new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = "PaymentSucceeded",
                    Payload = payment.Id.ToString(),
                    Status = OutboxStatus.Pending,
                    RetryCount = 0,
                });

                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();

            });

        }

        



       
      
        // =========================
        // FAILURE FLOW
        // =========================
        public async Task HandleFailureAsync(PaymentProvider provider, string payload)
        {
            var strategy = _unitOfWork.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _unitOfWork.BeginTransactionAsync();

                try
                {

                    var (providerRefId, eventId, rawData) = ExtractProviderData(provider, payload);

                    var paymentRepo = _unitOfWork.GetRepository<Payment, Guid>();
                    var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
                    var reservationRepo = _unitOfWork.GetRepository<SlotReservation, Guid>();
                    var slotRepo = _unitOfWork.GetRepository<AvailableSlots, Guid>();

                    var payment = await paymentRepo
                        .GetByIdAsync(new GetPaymentByProviderRefSpec(providerRefId));

                    if (payment == null)
                        return;

                    if (payment.Status == PaymentStatus.Failed)
                        return;

                    payment.Status = PaymentStatus.Failed;
                    payment.FailureReason = $"{provider} payment failed";
                    payment.RawResponseJson = rawData;

                    var order = await orderRepo.GetByIdAsync(payment.OrderId);

                    if (order != null)
                        order.Status = OrderStatus.PendingPayment;

                    //var reservations = await reservationRepo
                    //    .GetAllAsync(new GetAllSlotReservationWithOrderIdSpecification(payment.OrderId));

                    //foreach (var r in reservations)
                    //    r.Status = ReservationStatus.Pending;

                   
                  

                    await _unitOfWork.SaveChangesAsync();
                    await tx.CommitAsync();
                }
                catch {
                    await tx.RollbackAsync();
                        throw;

                }
            });
        }

       



        private (string providerRefId, string eventId, string raw)
            ExtractProviderData(PaymentProvider provider, string payload)
        {
            using var json = JsonDocument.Parse(payload);

            if (provider == PaymentProvider.Stripe)
            {
                var root = json.RootElement;

                var eventId = root
                    .GetProperty("id")
                    .GetString(); 

                var paymentId = root
                    .GetProperty("data")
                    .GetProperty("object")
                    .GetProperty("id")
                    .GetString(); 

                return (paymentId!, eventId!, payload);


            }
            if (provider == PaymentProvider.Paypal)
            {
                var root = json.RootElement;

                var eventId = root
                    .GetProperty("id")
                    .GetString(); 

                var paymentId = root
                    .GetProperty("resource")
                    .GetProperty("id")
                    .GetString(); 

                return (paymentId!, eventId!, payload);
            }

            throw new Exception("Unsupported provider");
        }

        public async Task HandleRefundCompleted(JsonElement root)
        {
            try
            {
                var resource =
                    root.GetProperty("resource");

                // PAYPAL REFUND ID

                var providerRefundId =
                    resource.GetProperty("id")
                        .GetString();
                var slotRepo =
                        _unitOfWork.GetRepository<
                            AvailableSlots,
                            Guid>();
                if (string.IsNullOrWhiteSpace(providerRefundId))
                {
                    _logger.LogWarning(
                        "Webhook refund id is missing");

                    return;
                }

                // GET REFUND

                var refundRepo =
                    _unitOfWork.GetRepository<Refund, Guid>();

                var refund =
                    await refundRepo.GetByIdAsync(new GetRefundWithProviderRefundIdSpecification(providerRefundId));
                            

                if (refund == null)
                {
                    _logger.LogWarning(
                        "Refund not found for ProviderRefundId: {ProviderRefundId}",
                        providerRefundId);

                    return;
                }

                // IDEMPOTENCY

                if (refund.Status ==
                    RefundStatus.Completed)
                {
                    _logger.LogInformation(
                        "Refund already completed: {RefundId}",
                        refund.Id);

                    return;
                }

                // STATUS FROM PAYPAL

                var status =
                    resource.GetProperty("status")
                        .GetString();

                if (status != "COMPLETED")
                {
                    _logger.LogWarning(
                        "Refund webhook status is not completed => {Status}",
                        status);

                    return;
                }

                // UPDATE REFUND

                refund.Status =
                    RefundStatus.Completed;

                refund.RefundedAt =
                    DateTime.UtcNow;

                refund.ProviderResponseJson =
                    root.ToString();


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

                     
                    }
                }
                else
                {
                    _logger.LogWarning(
                        "Available slot not found for RefundId: {RefundId}, SlotId: {SlotId}",
                        refund.Id,
                        refund.Booking.OrderItem.SlotId);

                  
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Refund webhook processed successfully => RefundId: {RefundId}",
                    refund.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing refund webhook");
            }
        }
    }
}

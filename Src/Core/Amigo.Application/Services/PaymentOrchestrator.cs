using Amigo.Application.Specifications.AvailableSlotsSpecification;
using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Application.Specifications.CartSpecification;
using Amigo.Application.Specifications.OrderSpecification;
using Amigo.Application.Specifications.PaymentSpecification;
using Amigo.Application.Specifications.Travelers;
using Amigo.Application.Specifications.WebhookEventLogSpecification;
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

        public PaymentOrchestrator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        // SUCCESS FLOW


        public async Task HandleSuccessAsync(PaymentProvider provider, string payload)
        {
            var (providerRefId,eventId ,rawData ) = ExtractProviderData(provider, payload);
            
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



        
    }
}

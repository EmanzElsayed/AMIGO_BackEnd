using Amigo.Application.Specifications.AvailableSlotsSpecification;
using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Application.Specifications.OrderSpecification;
using Amigo.Application.Specifications.PaymentSpecification;
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

        // =========================
        // SUCCESS FLOW
        // =========================
        public async Task HandleSuccessAsync(PaymentProvider provider, string payload)
        {
            var strategy = _unitOfWork.CreateExecutionStrategy();
           
            await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _unitOfWork.BeginTransactionAsync();


                try
                {
                    var (providerRefId, rawData) = ExtractProviderData(provider, payload);

                    var paymentRepo = _unitOfWork.GetRepository<Payment, Guid>();
                    var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
                    var reservationRepo = _unitOfWork.GetRepository<SlotReservation, Guid>();
                    var slotRepo = _unitOfWork.GetRepository<AvailableSlots, Guid>();
                    var bookingRepo = _unitOfWork.GetRepository<Booking, Guid>();

                    // 1. Payment
                    var payment = await paymentRepo
                        .GetByIdAsync(new GetPaymentByProviderRefSpec(providerRefId));

                    if (payment is null || payment.Status == PaymentStatus.Succeeded)
                        return;

                    payment.Status = PaymentStatus.Succeeded;
                    payment.PaidAt = DateTime.UtcNow;
                    payment.RawResponseJson = rawData;

                    // 2. Order 
                    var order = await orderRepo.GetByIdAsync( new GetOrderByIdSpecification( payment.OrderId));

                    if (order is null)
                        return;

                    order.Status = OrderStatus.Confirmed;

                    // 3. Reservations 
                    var reservations = await reservationRepo
                        .GetAllAsync(new GetAllSlotReservationWithOrderIdSpecification(payment.OrderId));

                    foreach (var r in reservations)
                        r.Status = ReservationStatus.Confirmed;

                    // 4.Load ALL slots in ONE query
                    var slotIds = reservations
                        .Select(r => r.SlotId)
                        .Distinct()
                        .ToList();

                    var slots = await slotRepo.GetAllAsync(new GetSlotsByIdsSpecification(slotIds));

                    var slotDict = slots.ToDictionary(x => x.Id);

                    foreach (var r in reservations)
                    {
                        if (slotDict.TryGetValue(r.SlotId, out var slot))
                        {
                            slot.ReservedCount -= r.Quantity;
                            slot.BookedCount += r.Quantity;
                        }
                    }

                    // 5. Batch check bookings
                    var orderItemIds = order.OrderItems.Select(x => x.Id).ToList();

                    var existingBookings = await bookingRepo.GetAllAsync(
                        new GetBookingsByOrderItemIdsSpecification(orderItemIds));

                    var existingSet = existingBookings.Select(x => x.OrderItemId).ToHashSet();

                    foreach (var item in order.OrderItems)
                    {
                        if (existingSet.Contains(item.Id))
                            continue;

                        var booking = new Booking
                        {
                            Id = Guid.NewGuid(),
                            OrderItemId = item.Id,
                            OrderId = order.Id,
                            UserId = order.UserId,
                            PaymentId = payment.Id,
                            BookingNumber = GenerateBookingNumber(),
                            Status = BookingStatus.Confirmed,
                            ConfirmedAt = DateTime.UtcNow
                        };

                        await bookingRepo.AddAsync(booking);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await tx.CommitAsync();
                }
                catch
                {
                    await tx.RollbackAsync();

  
                }
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

                    var (providerRefId, rawData) = ExtractProviderData(provider, payload);

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

                    var reservations = await reservationRepo
                        .GetAllAsync(new GetAllSlotReservationWithOrderIdSpecification(payment.OrderId));

                    foreach (var r in reservations)
                        r.Status = ReservationStatus.Cancelled;

                    var slotIds = reservations
                       .Select(r => r.SlotId)
                       .Distinct()
                       .ToList();

                    var slots = await slotRepo.GetAllAsync(new GetSlotsByIdsSpecification(slotIds));

                    var slotDict = slots.ToDictionary(s => s.Id);

                    foreach (var r in reservations)
                    {
                        if (slotDict.TryGetValue(r.SlotId, out var slot))
                        {
                            slot.ReservedCount -= r.Quantity;
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await tx.CommitAsync();
                }
                catch {
                    await tx.RollbackAsync();

                }
            });
        }

        // =========================    
        // HELPER
        // =========================
        private (string providerRefId, string raw) ExtractProviderData(PaymentProvider provider, string payload)
        {
            if (provider == PaymentProvider.Stripe)
            {
                var json = JsonDocument.Parse(payload);
                var id = json.RootElement.GetProperty("data")
                    .GetProperty("object")
                    .GetProperty("id")
                    .GetString();

                return (id, payload);
            }

            if (provider == PaymentProvider.Paypal)
            {
                var json = JsonDocument.Parse(payload);
                var id = json.RootElement
                    .GetProperty("resource")
                    .GetProperty("supplementary_data")
                    .GetProperty("related_ids")
                    .GetProperty("order_id")
                    .GetString();

                return (id, payload);
            }

            throw new Exception("Unsupported provider");
        }

        private string GenerateBookingNumber()
        {
            return $"AMG-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services;

using Amigo.Application.Specifications.AvailableSlotsSpecification;
using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Application.Specifications.OrderSpecification;
using Amigo.Application.Specifications.PaymentSpecification;
using Amigo.Domain.Abstraction;
using Amigo.Domain.DTO.Payment;
using Stripe;

public class PaymentService(IUnitOfWork _unitOfWork) : IPaymentService
{
    public async Task<CreatePaymentResponseDTO> CreateStripePaymentAsync(Order order)
    {
        var service = new PaymentIntentService();

        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(order.TotalAmount * 100),
            Currency = "usd",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            },
            Metadata = new Dictionary<string, string>
            {
                { "orderId", order.Id.ToString() }
            }
        };

        var intent = await service.CreateAsync(options);

        return new CreatePaymentResponseDTO
       (
            PaymentIntentId : intent.Id,
            ClientSecret : intent.ClientSecret
        );
    }


    public async Task HandlePaymentFailed(Event stripeEvent)
    {
        var intent = stripeEvent.Data.Object as PaymentIntent;

        if (intent == null)
            return;

        var strategy = _unitOfWork.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var paymentRepo = _unitOfWork.GetRepository<Payment, Guid>();
                var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
                var reservationRepo = _unitOfWork.GetRepository<SlotReservation, Guid>();

                // 1. Get Payment by Stripe Intent
                var payment = await paymentRepo
                    .GetByIdAsync(new GetPaymentByPaymentIntentSpecification(intent.Id));

                if (payment == null)
                    return;

                // 🔐 IDMPOTENCY CHECK
                if (payment.Status == PaymentStatus.Failed)
                    return;

                // 2. Update Payment
                payment.Status = PaymentStatus.Failed;
                payment.FailureReason = intent.LastPaymentError?.Message;
                payment.RawResponseJson = intent.ToJson();

                // 3. Get Order
                var order = await orderRepo.GetByIdAsync(payment.OrderId);

                if (order != null)
                {
                    order.Status = OrderStatus.PendingPayment;
                }

                // 4. Get Reservations
                var reservations = await reservationRepo
                    .GetAllAsync(new GetAllSlotReservationWithOrderIdSpecification(payment.OrderId));

                foreach (var reservation in reservations)
                {
                    reservation.Status = ReservationStatus.Cancelled;
                }

                // 5. Release Slot Capacity
                var slotRepo = _unitOfWork.GetRepository<AvailableSlots, Guid>();

                foreach (var reservation in reservations)
                {
                    var slot = await slotRepo.GetByIdAsync(reservation.SlotId);

                    if (slot != null)
                    {
                        // release capacity safely
                        slot.ReservedCount -= reservation.Quantity;

                        if (slot.ReservedCount < 0)
                            slot.ReservedCount = 0;
                    }
                }

                // 6. Save ALL changes in ONE transaction
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });



    }

    public async Task HandlePaymentSucceeded(Event stripeEvent)
    {
        var intent = stripeEvent.Data.Object as PaymentIntent;

        if (intent == null)
            return;

        var strategy = _unitOfWork.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var paymentRepo = _unitOfWork.GetRepository<Payment, Guid>();
                var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
                var reservationRepo = _unitOfWork.GetRepository<SlotReservation, Guid>();
                var slotRepo = _unitOfWork.GetRepository<AvailableSlots, Guid>();
                var bookingRepo = _unitOfWork.GetRepository<Booking, Guid>();
                // 1. Get Payment
                var payment = await paymentRepo
                    .GetByIdAsync(new GetPaymentByPaymentIntentSpecification(intent.Id));

                if (payment == null)
                    return;

                //  IDMPOTENCY CHECK
                if (payment.Status == PaymentStatus.Succeeded)
                    return;

                // 2. Update Payment
                payment.Status = PaymentStatus.Succeeded;
                payment.PaidAt = DateTime.UtcNow;
                payment.RawResponseJson = intent.ToJson();

                // 3. Get Order To Confirm
                var order = await orderRepo.GetByIdAsync(new GetOrderByIdSpecification(payment.OrderId));

                if (order is not null)
                {
                    order.Status = OrderStatus.Confirmed;

                    foreach (var item in order.OrderItems)
                    {
                        var exists = await bookingRepo.AnyAsync(
                           new GetBookingByItemIdSpecification(item.Id));

                        if (exists)
                            continue;
                        int totalPeople = item.OrderedPrice.Sum(x => x.Quantity);
                        var booking = new Booking
                        {
                            Id = Guid.NewGuid(),
                            OrderItemId = item.Id,
                            UserId = order.UserId,
                            User = order.User,
                            OrderId = order.Id,
                            PaymentId = payment.Id,
                            CustomerName = order.User.FullName,
                            CustomerEmail = order.User.Email,
                            RequiredTravelersCount = totalPeople,
                            BookingNumber = GenerateBookingNumber(),

                            Status = BookingStatus.Confirmed,
                            ConfirmedAt = DateTime.UtcNow,
                        };

                        await bookingRepo.AddAsync(booking);
                    }
                }
                
                // 4. Get Reservations To confirm
                var reservations = await reservationRepo
                   .GetAllAsync(new GetAllSlotReservationWithOrderIdSpecification(payment.OrderId));


                foreach (var reservation in reservations)
                {
                    reservation.Status = ReservationStatus.Confirmed;
                }

                // 5. Update Slot Capacity (BOOKED)
                foreach (var reservation in reservations)
                {
                    var slot = await slotRepo.GetByIdAsync(reservation.SlotId);

                    if (slot != null)
                    {
                        // move from reserved → booked
                        slot.ReservedCount -= reservation.Quantity;
                        slot.BookedCount += reservation.Quantity;

                        if (slot.ReservedCount < 0)
                            slot.ReservedCount = 0;
                    }
                }

                // 6. Save all changes
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
    private string GenerateBookingNumber()
    {
        return $"AMG-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
    }
}

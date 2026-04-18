using Amigo.Application.Specifications.OrderSpecification;
using Amigo.Domain.Abstraction;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class BackGroundServices(IUnitOfWork _unitOfWork)
    {
        //public async Task ExpirePendingOrders()
        //{
        //    var limit = DateTime.UtcNow.AddMinutes(-30);

        //    var _orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        //    var orders = await _orderRepo.GetAllAsync(new GetOrderByStatusAndTimeSpecification(limit));

        //    foreach (var order in orders)
        //    {
        //        order.Status = OrderStatus.Expired;

        //        var reservations = await _reservationRepo
        //            .FindAsync(r => r.OrderId == order.Id);

        //        foreach (var reservation in reservations)
        //        {
        //            if (reservation.Status == ReservationStatus.Pending)
        //            {
        //                reservation.Status = ReservationStatus.Expired;

        //                var slot = await _slotRepo.GetByIdAsync(reservation.SlotId);

        //                if (slot != null)
        //                {
        //                    slot.ReservedCount -= reservation.Quantity;

        //                    if (slot.ReservedCount < 0)
        //                        slot.ReservedCount = 0;
        //                }
        //            }
        //        }
        //    }

        //    await _unitOfWork.SaveChangesAsync();
        //}
        //public async Task ExpireReservations()
        //{
        //    var now = DateTime.UtcNow;

        //    var reservations = await _reservationRepo.FindAsync(r =>
        //        r.Status == ReservationStatus.Pending &&
        //        r.ExpiresAt < now);

        //    foreach (var r in reservations)
        //    {
        //        r.Status = ReservationStatus.Expired;

        //        var slot = await _slotRepo.GetByIdAsync(r.SlotId);

        //        if (slot != null)
        //        {
        //            slot.ReservedCount -= r.Quantity;

        //            if (slot.ReservedCount < 0)
        //                slot.ReservedCount = 0;
        //        }
        //    }

        //    await _unitOfWork.SaveChangesAsync();
        //}
        //public async Task ReconcilePayments()
        //{
        //    var payments = await _paymentRepo.FindAsync(p =>
        //        p.Status == PaymentStatus.Pending &&
        //        p.CreatedAt < DateTime.UtcNow.AddMinutes(-10));

        //    foreach (var payment in payments)
        //    {
        //        var service = new PaymentIntentService();

        //        var intent = await service.GetAsync(payment.ProviderPaymentIntentId);

        //        if (intent.Status == "succeeded")
        //        {
        //            payment.Status = PaymentStatus.Succeeded;
        //        }
        //        else if (intent.Status == "canceled")
        //        {
        //            payment.Status = PaymentStatus.Failed;
        //        }
        //    }

        //    await _unitOfWork.SaveChangesAsync();
        //}
        //public async Task RecalculateSlots()
        //{
        //    var slots = await _slotRepo.GetAllAsync();

        //    foreach (var slot in slots)
        //    {
        //        var reservations = await _reservationRepo.FindAsync(r =>
        //            r.SlotId == slot.Id &&
        //            r.Status == ReservationStatus.Confirmed);

        //        var reserved = await _reservationRepo.FindAsync(r =>
        //            r.SlotId == slot.Id &&
        //            r.Status == ReservationStatus.Pending);

        //        slot.BookedCount = reservations.Sum(r => r.Quantity);
        //        slot.ReservedCount = reserved.Sum(r => r.Quantity);
        //    }

        //    await _unitOfWork.SaveChangesAsync();
        //}
        //public async Task Cleanup()
        //{
        //    var old = DateTime.UtcNow.AddDays(-30);

        //    var expired = await _reservationRepo.FindAsync(r =>
        //        r.Status == ReservationStatus.Expired &&
        //        r.CreatedAt < old);

        //    _reservationRepo.RemoveRange(expired);

        //    await _unitOfWork.SaveChangesAsync();
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Validation.Common.Specifications
{
    public static class OrderCommonSpecifciation
    {
        public static Expression<Func<Order, bool>> BuildCriteriaForUser
       (GetAllOrdersQuery query, string userId)
        {

            var tourName = string.IsNullOrWhiteSpace(query.TourTitle) ? "": query.TourTitle.Trim().ToLower() ;
            
            OrderStatus? orderStatus = string.IsNullOrWhiteSpace(query.OrderStatus)
                ? null
                : EnumsMapping.ToEnum<OrderStatus>(query.OrderStatus, false);

            PaymentStatus? paymentStatus = string.IsNullOrWhiteSpace(query.PaymentStatus)
                        ? null
                    : EnumsMapping.ToEnum<PaymentStatus>(query.PaymentStatus, false);

            BookingStatus? bookingStatus = string.IsNullOrWhiteSpace(query.BookingStatus)
                       ? null
                   : EnumsMapping.ToEnum<BookingStatus>(query.BookingStatus, false);

            return o => 
                    o.UserId == userId && !o.IsDeleted  
                    &&

                  (string.IsNullOrWhiteSpace(query.OrderStatus) || o.Status == orderStatus)
                  &&
                  (query.OrderDate == null || o.OrderDate == query.OrderDate)

                  && (string.IsNullOrWhiteSpace(query.TourTitle) ||

                  o.OrderItems.Any(i =>
                      !string.IsNullOrWhiteSpace(i.TourTitle) &&
                      i.TourTitle.Trim().ToLower().Contains(tourName)
                  ))

                 && (string.IsNullOrWhiteSpace(query.PaymentStatus) || o.Payments.Any(p => !p.IsDeleted && p.Status == paymentStatus))

                && (string.IsNullOrWhiteSpace(query.BookingStatus) || o.OrderItems.Any(o => !o.IsDeleted && o.Booking != null && o.Booking.Status == bookingStatus))

                && (query.OrderExpiresAt == null || o.ExpiresAt == query.OrderExpiresAt)
                && (query.BookingNumber == null || o.OrderItems.Any(o => !o.IsDeleted && o.Booking != null && o.Booking.BookingNumber == query.BookingNumber));

        }
    }
}

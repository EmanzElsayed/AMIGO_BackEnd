using Amigo.Domain.DTO.Order;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Amigo.Application.Mapping
{
    public static class OrderMapping
    {
        public static List<OrderDetailsDTO> ToDTOs(this IEnumerable<Order> orders)
        {
            return orders.Select(o => new OrderDetailsDTO(
                     OrderId: o.Id,
                     Status: o.Status.ToString(),
                     TotalAmount: o.TotalAmount,
                     Currency: o.Currency.ToString(),
                     OrderDate: o.OrderDate,
                     ExpiresAt:o.ExpiresAt,
                     Payments: o.Payments.Select(p => new PaymentResponseDTO(
                         PaymentId : p.Id,
                         PaidAmount:p.TotalAmount,
                         PaymentMethod:p.PaymentMethod.ToString(),
                         PaymentStatus:p.Status.ToString(),
                         PaidCurrency:p.Currency.ToString(),
                         PaidAt:p.PaidAt,
                         Provider:p.Provider.ToString(),
                         FailureReason:p.FailureReason


                         )).ToList(),
                     Items: o.OrderItems.Select(item => new OrderItemDetailsDTO(
                            OrderItemId: item.Id,
                            BookingNumber: item.Booking is null ? "" : item.Booking.BookingNumber,
                             BookingStatus: item.Booking is null ? "" : item.Booking.Status.ToString(),

                            TourId: item.TourId,
                            TourTitle: item.TourTitle,
                            DestinationName: item.DestinationName,
                            TourDate: item.TourDate,
                            StartTime: item.StartTime,
                            MeetingPoint: item.MeetingPoint,
                            CancelationPolicyType: item.CancelationPolicyType.ToString(),
                            CancellationBefore: item.CancellationBefore,
                            RefundPercentage: item.RefundPercentage,

                            Prices: item.OrderedPrice.Select(price => new OrderedPricesResponseDTO(

                                    PriceId: price.Id,
                                    Type: price.Type,
                                    RetailPrice: price.RetailPrice,
                                    Quantity: price.Quantity,
                                    FinalPrice: price.FinalPrice
                            )).ToList()
                     )).ToList()


            )).ToList();
        
        }





        public static OrderDetailsDTO ToDTO(this Order o)
        {
            return new OrderDetailsDTO( 

                  OrderId: o.Id,
                     Status: o.Status.ToString(),
                     TotalAmount: o.TotalAmount,
                     Currency: o.Currency.ToString(),
                     OrderDate: o.OrderDate,
                      ExpiresAt: o.ExpiresAt,
                     Payments: o.Payments.Select(p => new PaymentResponseDTO(
                         PaymentId: p.Id,
                         PaidAmount: p.TotalAmount,
                         PaymentMethod: p.PaymentMethod.ToString(),
                         PaymentStatus: p.Status.ToString(),
                         PaidCurrency: p.Currency.ToString(),
                         PaidAt: p.PaidAt,
                         Provider: p.Provider.ToString(),
                         FailureReason: p.FailureReason


                         )).ToList(),
                     Items: o.OrderItems.Select(item => new OrderItemDetailsDTO(
                            OrderItemId: item.Id,
                            BookingNumber: item.Booking is null ? "" : item.Booking.BookingNumber,
                             BookingStatus: item.Booking is null ? "" : item.Booking.Status.ToString(),
                            TourId: item.TourId,
                            TourTitle: item.TourTitle,
                            DestinationName: item.DestinationName,
                            TourDate: item.TourDate,
                            StartTime: item.StartTime,
                            MeetingPoint: item.MeetingPoint,
                            CancelationPolicyType: item.CancelationPolicyType.ToString(),
                            CancellationBefore: item.CancellationBefore,
                            RefundPercentage: item.RefundPercentage,
                            Prices: item.OrderedPrice.Select(price => new OrderedPricesResponseDTO(

                                    PriceId: price.Id,
                                    Type: price.Type,
                                    RetailPrice: price.RetailPrice,
                                    Quantity: price.Quantity,
                                    FinalPrice: price.FinalPrice
                            )).ToList()
                     )).ToList()

            );
        
        }

        public static List<OrderDetailsForAdminResponseDTO> ToDTOsForAdmin(this IEnumerable<Order> orders)
        {
            return orders.Select(o => new OrderDetailsForAdminResponseDTO(
                     OrderId: o.Id,
                     UserName:o.User.FullName,
                     UserEmail:o.User.Email,
                     Status: o.Status.ToString(),
                     TotalAmount: o.TotalAmount,
                     Currency: o.Currency.ToString(),
                     OrderDate: o.OrderDate,
                     ExpiresAt: o.ExpiresAt,
                     Payments: o.Payments.Select(p => new PaymentResponseDTO(
                         PaymentId: p.Id,
                         PaidAmount: p.TotalAmount,
                         PaymentMethod: p.PaymentMethod.ToString(),
                         PaymentStatus: p.Status.ToString(),
                         PaidCurrency: p.Currency.ToString(),
                         PaidAt: p.PaidAt,
                         Provider: p.Provider.ToString(),
                         FailureReason: p.FailureReason


                         )).ToList(),
                     Items: o.OrderItems.Select(item => new OrderItemDetailsDTO(
                            OrderItemId: item.Id,
                            BookingNumber: item.Booking is null ? "" : item.Booking.BookingNumber,
                             BookingStatus: item.Booking is null ? "" : item.Booking.Status.ToString(),

                            TourId: item.TourId,
                            TourTitle: item.TourTitle,
                            DestinationName: item.DestinationName,
                            TourDate: item.TourDate,
                            StartTime: item.StartTime,
                            MeetingPoint: item.MeetingPoint,
                            CancelationPolicyType: item.CancelationPolicyType.ToString(),
                            CancellationBefore: item.CancellationBefore,
                            RefundPercentage: item.RefundPercentage,

                            Prices: item.OrderedPrice.Select(price => new OrderedPricesResponseDTO(

                                    PriceId: price.Id,
                                    Type: price.Type,
                                    RetailPrice: price.RetailPrice,
                                    Quantity: price.Quantity,
                                    FinalPrice: price.FinalPrice
                            )).ToList()
                     )).ToList()


            )).ToList();

        }


        public static OrderDetailsForAdminResponseDTO ToDTOForAdmin(this Order o)
        {
            return new OrderDetailsForAdminResponseDTO(

                    OrderId: o.Id,
                    UserName: o.User.FullName,
                     UserEmail: o.User.Email,
                     Status: o.Status.ToString(),
                     TotalAmount: o.TotalAmount,
                     Currency: o.Currency.ToString(),
                     OrderDate: o.OrderDate,
                      ExpiresAt: o.ExpiresAt,
                     Payments: o.Payments.Select(p => new PaymentResponseDTO(
                         PaymentId: p.Id,
                         PaidAmount: p.TotalAmount,
                         PaymentMethod: p.PaymentMethod.ToString(),
                         PaymentStatus: p.Status.ToString(),
                         PaidCurrency: p.Currency.ToString(),
                         PaidAt: p.PaidAt,
                         Provider: p.Provider.ToString(),
                         FailureReason: p.FailureReason


                         )).ToList(),
                     Items: o.OrderItems.Select(item => new OrderItemDetailsDTO(
                            OrderItemId: item.Id,
                            BookingNumber: item.Booking is null ? "" : item.Booking.BookingNumber,
                             BookingStatus: item.Booking is null ? "" : item.Booking.Status.ToString(),
                            TourId: item.TourId,
                            TourTitle: item.TourTitle,
                            DestinationName: item.DestinationName,
                            TourDate: item.TourDate,
                            StartTime: item.StartTime,
                            MeetingPoint: item.MeetingPoint,
                            CancelationPolicyType: item.CancelationPolicyType.ToString(),
                            CancellationBefore: item.CancellationBefore,
                            RefundPercentage: item.RefundPercentage,
                            Prices: item.OrderedPrice.Select(price => new OrderedPricesResponseDTO(

                                    PriceId: price.Id,
                                    Type: price.Type,
                                    RetailPrice: price.RetailPrice,
                                    Quantity: price.Quantity,
                                    FinalPrice: price.FinalPrice
                            )).ToList()
                     )).ToList()

            );

        }

    }
}

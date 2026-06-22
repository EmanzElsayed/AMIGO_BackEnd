using Amigo.Application.Helpers;
using Amigo.Domain.DTO.Cancellation;
using Amigo.Domain.DTO.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Amigo.Application.Mapping
{
    public static class OrderMapping
    { 
        public static List<OrderDetailsDTO> ToDTOs(this IEnumerable<Order> orders,Dictionary<Guid,string?>? tourImages)
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
                            BookingId: item.Booking is null ? null: item.Booking.Id,
                            BookingNumber: item.Booking is null ? "" : item.Booking.BookingNumber,
                             BookingStatus: item.Booking is null ? "" : item.Booking.Status.ToString(),

                            TourId: item.TourId,
                            TourImage: tourImages is null || item.TourId is null ? null : ( tourImages.TryGetValue(item.TourId.Value, out var image)
                                                ? image : null
                                                ),
                            TourTitle: item.TourTitle,
                            TourSlug: SlugHelper.ToUrlSlug(item.TourTitle),
                            DestinationName: item.DestinationName,
                            TourDate: item.TourDate,
                            StartTime: item.StartTime,
                            MeetingPoint: item.MeetingPoint,
                            CancellationPloicy : item.CancellationPolicies is null || !item.CancellationPolicies.Any() ? null:  item.CancellationPolicies.Select(c => new GetCancellationResponseDTO(
                                Id : c.Id,
                                CancelationPolicyType: c.CancelationPolicyType.ToString(),
                            CancellationBefore: c.CancellationBefore,
                            RefundPercentage: c.RefundPercentage

                                )).ToList(),
                            
                            Prices: item.OrderedPrice.Select(price => new OrderedPricesResponseDTO(

                                    PriceId: price.Id,
                                    Type: price.Type,
                                    ConvertedRetailPrice: price.ConvertedRetailPrice,
                                    Quantity: price.Quantity,
                                    FinalPrice: price.FinalPrice
                            )).ToList()
                     )).ToList()


            )).ToList();
        
        }





        public static OrderDetailsDTO ToDTO(this Order o, Dictionary<Guid, string?>? tourImages)
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
                           BookingId: item.Booking is null ? null : item.Booking.Id,
                            BookingNumber: item.Booking is null ? "" : item.Booking.BookingNumber,
                             BookingStatus: item.Booking is null ? "" : item.Booking.Status.ToString(),
                            TourId: item.TourId,
                             TourImage: tourImages is null || item.TourId is null ? null : (tourImages.TryGetValue(item.TourId.Value, out var image)
                                                ? image : null
                                                ),
                            TourTitle: item.TourTitle,
                             TourSlug: SlugHelper.ToUrlSlug(item.TourTitle),
                            DestinationName: item.DestinationName,
                            TourDate: item.TourDate,
                            StartTime: item.StartTime,
                            MeetingPoint: item.MeetingPoint,
                             CancellationPloicy: item.CancellationPolicies is null || !item.CancellationPolicies.Any() ? null : item.CancellationPolicies.Select(c => new GetCancellationResponseDTO(
                                Id: c.Id,
                                CancelationPolicyType: c.CancelationPolicyType.ToString(),
                            CancellationBefore: c.CancellationBefore,
                            RefundPercentage: c.RefundPercentage

                                )).ToList(),
                            Prices: item.OrderedPrice.Select(price => new OrderedPricesResponseDTO(

                                    PriceId: price.Id,
                                    Type: price.Type,
                                    ConvertedRetailPrice: price.BaseRetailPrice,
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
                            BookingId : item.Booking is null ? null : item.Booking.Id,
                            BookingNumber: item.Booking is null ? "" : item.Booking.BookingNumber,
                             BookingStatus: item.Booking is null ? "" : item.Booking.Status.ToString(),

                            TourId: item.TourId,
                            TourTitle: item.TourTitle,
                            TourSlug: SlugHelper.ToUrlSlug(item.TourTitle),
                            DestinationName: item.DestinationName,
                            TourDate: item.TourDate,
                            StartTime: item.StartTime,
                            MeetingPoint: item.MeetingPoint,
                             CancellationPloicy: item.CancellationPolicies is null || !item.CancellationPolicies.Any() ? null : item.CancellationPolicies.Select(c => new GetCancellationResponseDTO(
                                Id: c.Id,
                                CancelationPolicyType: c.CancelationPolicyType.ToString(),
                            CancellationBefore: c.CancellationBefore,
                            RefundPercentage: c.RefundPercentage

                                )).ToList(),

                            Prices: item.OrderedPrice.Select(price => new OrderedPricesResponseDTO(

                                    PriceId: price.Id,
                                    Type: price.Type,
                                    ConvertedRetailPrice: price.BaseRetailPrice,
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
                            BookingId: item.Booking is null ? null : item.Booking.Id,
                            BookingNumber: item.Booking is null ? "" : item.Booking.BookingNumber,
                             BookingStatus: item.Booking is null ? "" : item.Booking.Status.ToString(),
                            TourId: item.TourId,
                            TourTitle: item.TourTitle,
                            TourSlug: SlugHelper.ToUrlSlug(item.TourTitle),
                            DestinationName: item.DestinationName,
                            TourDate: item.TourDate,
                            StartTime: item.StartTime,
                            MeetingPoint: item.MeetingPoint,
                             CancellationPloicy: item.CancellationPolicies is null || !item.CancellationPolicies.Any() ? null : item.CancellationPolicies.Select(c => new GetCancellationResponseDTO(
                                Id: c.Id,
                                CancelationPolicyType: c.CancelationPolicyType.ToString(),
                            CancellationBefore: c.CancellationBefore,
                            RefundPercentage: c.RefundPercentage

                                )).ToList(),
                            Prices: item.OrderedPrice.Select(price => new OrderedPricesResponseDTO(

                                    PriceId: price.Id,
                                    Type: price.Type,
                                    ConvertedRetailPrice: price.BaseRetailPrice,
                                    Quantity: price.Quantity,
                                    FinalPrice: price.FinalPrice
                            )).ToList()
                     )).ToList()

            );

        }

    }
}

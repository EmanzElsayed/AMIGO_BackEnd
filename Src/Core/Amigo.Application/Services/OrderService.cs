//using Amigo.Application.Specifications.BookingSpecification;
//using Amigo.Application.Specifications.OrderSpecification;
//using Amigo.Domain.Abstraction;
//using Amigo.Domain.DTO.Order;
//using Amigo.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Amigo.Application.Services
//{
//    public class OrderService(IUnitOfWork _unitOfWork):IOrderService
//    {
//        public async Task<Result<OrderDetailsDTO>> GetOrderDetailsAsync(
//     Guid orderId,
//     string userId)
//        {
//            var repo = _unitOfWork.GetRepository<Order, Guid>();

//            var order = await repo.GetByIdAsync(
//                new GetOrderByIdSpecification(orderId));

//            if (order is null)
//                return Result.Fail("Order not found");

//            // Authorization
//            if (order.UserId != userId)
//                return Result.Fail("Unauthorized");

//            var bookingRepo = _unitOfWork.GetRepository<Booking, Guid>();

//            var items = new List<OrderItemDetailsDTO>();

//            foreach (var item in order.OrderItems)
//            {
//                var booking = await bookingRepo.GetByIdAsync(
//                    new GetBookingByItemIdSpecification(item.Id));

//                items.Add(new OrderItemDetailsDTO(
//                    OrderItemId: item.Id,
//                    TourTitle: item.TourTitle,
//                    TourDate: item.TourDate,
//                    StartTime: item.StartTime,
                  
//                    BookingId: booking?.Id,
//                    BookingNumber: booking?.BookingNumber
//                ));
                
//            }

//            return Result.Ok(new OrderDetailsDTO(
//                OrderId: order.Id,
//                Status: order.Status.ToString(),
//                TotalAmount: order.TotalAmount,
//                Currency: order.Currency.ToString(),
//                OrderDate: order.OrderDate,
//                Items: items
//            ));
//        }
//    }
//}

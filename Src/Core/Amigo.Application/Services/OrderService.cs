using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Application.Specifications.ImagesSpecification;
using Amigo.Application.Specifications.OrderSpecification;
using Amigo.Domain.Abstraction;
using Amigo.Domain.DTO.Order;
using Amigo.Domain.Entities;
using Amigo.SharedKernal.DTOs.Tour;
using PayPalCheckoutSdk.Orders;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Order = Amigo.Domain.Entities.Order;

namespace Amigo.Application.Services
{
    public class OrderService(IUnitOfWork _unitOfWork, IValidationService _validationService) : IOrderService
    {
        public async Task<Result<PaginatedResponse<OrderDetailsDTO>>> GetAllOrders(string userId, GetAllOrdersQuery query)
        {
            var validationResult = await _validationService.ValidateAsync(query);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var orderSpec = new GetAllOrdersSpecification(userId, query);
            var orders = await orderRepo.GetAllAsync(orderSpec);
            
            var totalItems = await orderRepo.GetCountSpecificationAsync(new GetCountOfOrdersSpecification(userId, query));




            var mappedOrders = orders.ToDTOs();

            var totalPages = query.PageSize <= 0
                ? 0
                : (int)Math.Ceiling(totalItems / (double)query.PageSize);

            var response = new PaginatedResponse<OrderDetailsDTO>
            {
                Data = mappedOrders,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
            };
            return Result.Ok(response);

        }

        public async Task<Result<OrderDetailsDTO>> GetOrderDetailsAsync(string Id, string userId)
        {
            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid OrderId = guid;
            var order = await _unitOfWork.GetRepository<Domain.Entities.Order, Guid>().GetByIdAsync(new GetNotDeletedOrderByIdSpecification(OrderId));

            if (order is null)
            {
                return Result.Fail(new NotFoundError("This Order Not Found"));

            }
            if (order.UserId != userId)
            {
                return Result.Fail(new UnauthorizedError("Unauthorized"));

            }


            var tourIds = order.OrderItems.Where(i => i.TourId.HasValue).Select(i => i.TourId!.Value).Distinct().ToList();
            var imageRepo = _unitOfWork.GetRepository<TourImage, Guid>();
            var images = await imageRepo.GetAllAsync(new GetAllImagesWithToursIdsSpecification(tourIds));

            var imageMap = images
              .GroupBy(img => img.TourId)
              .ToDictionary(g => g.Key, g => g.OrderBy(x => x.CreatedDate).Select(x => x.ImageUrl).FirstOrDefault());


            var mappedOrder = order.ToDTO();

            return Result.Ok(mappedOrder);

        }
    }
}

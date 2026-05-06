using Amigo.Application.Abstraction.Services;
using Amigo.Application.Specifications.ImagesSpecification;
using Amigo.Application.Specifications.OrderSpecification;
using Amigo.Domain.Abstraction;
using Amigo.Domain.DTO.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.Admin
{
    public class AdminOrderService(IUnitOfWork _unitOfWork,
                    IValidationService _validationService)
                    : IAdminOrderService
    {
        public async Task<Result<PaginatedResponse<OrderDetailsForAdminResponseDTO>>> GetAllOrders(GetAllAdminOrdersQuery query)
        {
            var validationResult = await _validationService.ValidateAsync(query);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var orderSpec = new GetAllOrdersForAdminSpecification(query);
            var orders = await orderRepo.GetAllAsync(orderSpec);

            var totalItems = await orderRepo.GetCountSpecificationAsync(new GetCountOfOrdersForAdminSpecification(query));




            var mappedOrders = orders.ToDTOsForAdmin();

            var totalPages = query.PageSize <= 0
                ? 0
                : (int)Math.Ceiling(totalItems / (double)query.PageSize);

            var response = new PaginatedResponse<OrderDetailsForAdminResponseDTO>
            {
                Data = mappedOrders,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
            };
            return Result.Ok(response);

        }

        public async Task<Result<OrderDetailsForAdminResponseDTO>> GetOrderDetailsAsync(string Id)
        {
            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid OrderId = guid;
            var order = await _unitOfWork.GetRepository<Domain.Entities.Order, Guid>().GetByIdAsync(new GetNotDeletedOrderByIdForAdminSpecification(OrderId));

            if (order is null)
            {
                return Result.Fail(new NotFoundError("This Order Not Found"));

            }
          

        

            var mappedOrder = order.ToDTOForAdmin();

            return Result.Ok(mappedOrder);
        }
    }
}

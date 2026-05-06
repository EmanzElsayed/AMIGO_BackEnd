using Amigo.Domain.DTO.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services.Admin
{
    public interface IAdminOrderService
    {
        Task<Result<OrderDetailsForAdminResponseDTO>> GetOrderDetailsAsync(
           string orderId
           );

        Task<Result<PaginatedResponse<OrderDetailsForAdminResponseDTO>>> GetAllOrders(GetAllAdminOrdersQuery query);
    }
}

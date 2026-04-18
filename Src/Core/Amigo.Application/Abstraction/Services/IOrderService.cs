using Amigo.Domain.DTO.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IOrderService
    {
        Task<Result<OrderDetailsDTO>> GetOrderDetailsAsync(
            Guid orderId,
            string userId);
    }
}

using Amigo.Application.Abstraction.Services;
using Amigo.Application.Services;
using Amigo.Domain.Errors.BusinessErrors;
using Amigo.SharedKernal.QueryParams;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Amigo.Presentation.Controllers.User
{
    [Route("api/v1/user/order")]

    public class OrderController(IOrderService _orderService) :BaseController
    {
        [HttpGet]
        [Authorize]
        public async Task<IResultBase> getAllOrders([FromQuery] GetAllOrdersQuery query)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Result.Fail(new UnauthorizedError("Unauthorized"));
            }
            return await _orderService.GetAllOrders(userId,query);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IResultBase> getOrderById(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Result.Fail(new UnauthorizedError("Unauthorized"));
            }
            return await _orderService.GetOrderDetailsAsync(id,userId);
        }
    }
}

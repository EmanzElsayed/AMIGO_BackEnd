using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Domain.Errors.BusinessErrors;
using Amigo.SharedKernal.QueryParams;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Amigo.Presentation.Controllers.Admin
{
    [Route("api/v1/admin/order")]
    [Authorize(Roles = "Admin")]
    public class AdminOrderController(IAdminOrderService _adminOrderService):BaseController
    {
        [HttpGet]
        public async Task<IResultBase> getAllOrders([FromQuery] GetAllAdminOrdersQuery query)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Result.Fail(new UnauthorizedError("Unauthorized"));
            }
            return await _adminOrderService.GetAllOrders(query);
        }

        [HttpGet("{id}")]
        public async Task<IResultBase> getOrderById(string id)
        {
           
            return await _adminOrderService.GetOrderDetailsAsync(id);
        }

    }
}

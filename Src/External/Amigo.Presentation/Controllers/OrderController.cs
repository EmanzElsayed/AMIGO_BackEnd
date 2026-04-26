using Amigo.Application.Abstraction.Services;
using Microsoft.AspNetCore.Authorization;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1/order")]

    public class OrderController(IOrderService _orderService):BaseController
    {
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IResultBase> GetOrder(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return await _orderService.GetOrderDetailsAsync(id, userId);
        }
        
    }
}

using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Cart;
using Amigo.SharedKernal.QueryParams;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Amigo.Presentation.Controllers.User
{
    [Route("api/v1/user/cart")]

    public class CartController(IServiceManager _serviceManager) : BaseController
    {
        [EnableRateLimiting("token")]
        [HttpGet]
        public async Task<IResultBase> GetOrCreateCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartToken = Request.Headers["X-Cart-Token"].FirstOrDefault();
            return await _serviceManager.CartService.GetCurrentCartAsync(userId, cartToken);
        }
        [EnableRateLimiting("token")]
        [HttpPost("item")]
        public async Task<IResultBase> AddItem([FromBody] AddCartItemRequestDTO requestDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartToken = Request.Headers["X-Cart-Token"].FirstOrDefault();
            return await _serviceManager.CartService.AddItemAsync(userId, cartToken, requestDTO);
        }
        [EnableRateLimiting("token")]
        [HttpPost("item/{id}")]
        public async Task<IResultBase> UpdateItem(Guid id,[FromBody] UpdateCartItemRequestDTO requestDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartToken = Request.Headers["X-Cart-Token"].FirstOrDefault();
            return await _serviceManager.CartService.UpdateItemAsync(id,userId, cartToken, requestDTO);
        }

        [EnableRateLimiting("booking")]
        [HttpPost("checkout")]
        public async Task<IResultBase> Checkout([FromBody] CheckoutRequestDTO requestDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartToken = Request.Headers["X-Cart-Token"].FirstOrDefault();
            return await _serviceManager.CartService.CheckoutAsync(requestDTO, userId, cartToken);
        }


        [EnableRateLimiting("token")]
        [HttpDelete("item/{id}")]
        public async Task<IResultBase> RemoveItem(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartToken = Request.Headers["X-Cart-Token"].FirstOrDefault();

            return await _serviceManager.CartService.RemoveItemAsync(id, userId, cartToken);
        }
        [EnableRateLimiting("token")]
        [HttpDelete]
        public async Task<IResultBase> ClearCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartToken = Request.Headers["X-Cart-Token"].FirstOrDefault();

            return await _serviceManager.CartService.ClearAsync(userId, cartToken);
        }

        //[HttpPost("merge")]
        //    public Task<IActionResult> MergeGuestCart()

    }
}

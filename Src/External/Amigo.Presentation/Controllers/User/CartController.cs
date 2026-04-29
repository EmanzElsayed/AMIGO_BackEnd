using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Cart;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Amigo.Presentation.Controllers.User
{
    [Route("api/v1/user/cart")]

    public class CartController(ICartService _cartService) : BaseController
    {
        [HttpGet]
        public async Task<IResultBase> GetOrCreateCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartToken = Request.Headers["X-Cart-Token"].FirstOrDefault();
            return await _cartService.GetCurrentCartAsync(userId, cartToken);
        }
        [HttpPost("item")]
        public async Task<IResultBase> AddItem([FromBody] AddCartItemRequestDTO requestDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartToken = Request.Headers["X-Cart-Token"].FirstOrDefault();
            return await _cartService.AddItemAsync(userId, cartToken, requestDTO);
        }

        [HttpPost("item/{id}")]
        public async Task<IResultBase> UpdateItem(Guid id,[FromBody] UpdateCartItemRequestDTO requestDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartToken = Request.Headers["X-Cart-Token"].FirstOrDefault();
            return await _cartService.UpdateItemAsync(id,userId, cartToken, requestDTO);
        }
        [HttpPost("checkout")]
        public async Task<IResultBase> Checkout([FromBody] CheckoutRequestDTO requestDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartToken = Request.Headers["X-Cart-Token"].FirstOrDefault();
            return await _cartService.CheckoutAsync(requestDTO, userId, cartToken);
        }



        [HttpDelete("item/{id}")]
        public async Task<IResultBase> RemoveItem(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartToken = Request.Headers["X-Cart-Token"].FirstOrDefault();

            return await _cartService.RemoveItemAsync(id, userId, cartToken);
        }

        [HttpDelete]
        public async Task<IResultBase> ClearCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartToken = Request.Headers["X-Cart-Token"].FirstOrDefault();

            return await _cartService.ClearAsync(userId, cartToken);
        }

        //[HttpPost("merge")]
        //    public Task<IActionResult> MergeGuestCart()

    }
}

using Amigo.Domain.DTO.Cart;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface ICartService
    {
        Task<Result<CartDTO>> GetCurrentCartAsync(string? userId, string? cartToken);
        Task<Result<CartDTO>> AddItemAsync(
              string? userId,
              string? cartToken,
              AddCartItemRequestDTO requestDTO);


        Task<Result<CartDTO>> UpdateItemAsync(
            Guid itemId,
            string? userId,
            string? cartToken,
            UpdateCartItemRequestDTO dto);
        Task<Result<CheckoutResponseDTO>> CheckoutAsync(CheckoutRequestDTO requestDTO, string userId, string? cartToken);

        //Task<CartDto> RemoveItemAsync(
        //    Guid itemId,
        //    string? userId,
        //    string? cartToken);

        //Task ClearAsync(string? userId, string? cartToken);

        //Task<CartDto> MergeAsync(string userId, string guestCartToken);

    }
}

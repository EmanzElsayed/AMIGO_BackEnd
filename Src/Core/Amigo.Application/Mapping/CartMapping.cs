using Amigo.Domain.DTO.Cart;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public  static class CartMapping
    {
        public static CartDTO ToDto(this Cart cart)
        {
            
            return new CartDTO
            (
                
                Id : cart.Id,
                UserId : cart.UserId,
                CartToken: cart.CartToken,
                CurrencyCode: cart.CurrencyCode.ToString(),
                TotalAmount: cart.TotalAmount,
                TotalItems: cart.Items?.Count ?? 0,
                LastUpdatedAt: cart.LastUpdatedAt,
                ExpiresAt: cart.ExpiresAt,

                Items: cart.Items?
                    .Select(item => new CartItemDTO
                    (
                        Id : item.Id,
                        TourId : item.TourId,
                        SlotId : item.SlotId,
                        Language : item.Language,
                        TourDate: item.TourDate,
                        StartTime: item.StartTime,
                        TourName: item.TourTitle,
                        DestinationName: item.DestinationName,
                        TotalAmount: item.TotalAmount,

                        Prices: item.Prices?
                            .Select(price => new CartPriceDTO
                            (
                                Id: price.Id,
                                Type: price.Type,
                                RetailPrice: price.RetailPrice,
                                Quantity: price.Quantity,
                                FinalPrice: price.FinalPrice
                            ))
                            .ToList() ?? new List<CartPriceDTO>()
                    ))
                    .ToList() ?? new List<CartItemDTO>()
            );
        }
    }
}

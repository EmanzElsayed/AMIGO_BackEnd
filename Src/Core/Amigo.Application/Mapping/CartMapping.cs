using Amigo.Application.Services;
using Amigo.Domain.DTO.Cart;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public  static class CartMapping
    {
        public static CartDTO ToDto(this Cart cart, Dictionary<Guid, string> tourimages, EncryptionService _encryptionService)
        {

            return new CartDTO
            (

                Id: cart.Id,
                UserId: cart.UserId,
                CartToken: cart.CartToken,
                CurrencyCode: cart.CurrencyCode.ToString(),
                TotalAmount: cart.TotalAmount,
                TotalItems: cart.Items?.Count(i => !i.IsDeleted) ?? 0,
                LastUpdatedAt: cart.LastUpdatedAt,
                ExpiresAt: cart.ExpiresAt,

                Items: cart.Items?
                    .Where(item => !item.IsDeleted)
                    .Select(item =>
                    {
                        tourimages.TryGetValue(item.TourId, out string? imageUrl);
                        return new CartItemDTO
                       (
                           Id: item.Id,
                           TourId: item.TourId,
                           SlotId: item.SlotId ?? null,
                           Language: item.Language,
                           TourDate: item.TourDate,
                           StartTime: item.StartTime,
                           ImageUrl : imageUrl ?? null
                         ,
                           TourName: item.TourTitle,
                           DestinationName: item.DestinationName,
                           TotalAmount: item.TotalAmount,
                           ActivityType: item.ActivityType,
                           Prices: item.Prices?
                               .Select(price => new CartPriceDTO
                               (
                                   Id: price.Id,
                                   Type: price.Type,
                                   ConvertedRetailPrice: price.ConvertedRetailPrice,
                                   Quantity: price.Quantity,
                                   FinalPrice: price.FinalPrice
                               ))
                               .ToList() ?? new List<CartPriceDTO>(),

                           Travelers: item.Travelers?
                               .Select(t => new CheckoutTravelersRequestDTO
                               (
                                   Type: t.Type,
                                   FirstName: t.FullName.Split(' ', 2).FirstOrDefault() ?? "",
                                   LastName: t.FullName.Split(' ', 2).LastOrDefault() ?? "",
                                   Nationality: t.Nationality,
                                   PassportNumber: string.IsNullOrWhiteSpace(t.PassportNumber)? "":   _encryptionService.Decrypt(t.PassportNumber),
                                   BirthDate: t.BirthDate ?? DateOnly.FromDateTime(DateTime.Today)
                               ))
                               .ToList() ?? new List<CheckoutTravelersRequestDTO>()
                     );
                    }
                    )
                    .ToList() ?? new List<CartItemDTO>()
            );
        }
    }
}

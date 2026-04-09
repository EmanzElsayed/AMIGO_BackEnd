using Amigo.Domain.DTO.Price;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public class PriceMapping : IPriceMapping
    {
        public Price PriceDTOToEntity(CreatePriceRequestDTO requestDTO, Tour tour)
        {
            return new Price()
            {
                Id = Guid.NewGuid(),
                Tour = tour,
                TourId = tour.Id,
                Cost = requestDTO.Cost,
                Type = requestDTO.Type,
                Discount = requestDTO.Discount ?? 0,
                IsVip = tour.IsVip,
                IsPublic = tour.IsPublic
            };
        }
    }
}

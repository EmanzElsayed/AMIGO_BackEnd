using Amigo.Domain.DTO.Price;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.MappingInterfaces
{
    public interface IPriceMapping
    {
        Price PriceDTOToEntity(CreatePriceRequestDTO requestDTO , Tour tour);
    }
}

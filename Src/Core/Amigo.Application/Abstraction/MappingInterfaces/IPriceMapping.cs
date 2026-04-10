using Amigo.Domain.DTO.Price;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.MappingInterfaces
{
    public interface IPriceMapping
    {
        List<Price> PricesDTOToEntity(List<CreatePriceRequestDTO> requestDTO , Tour tour ,string language);

        //List<PriceTranslation> pricesTranslationToEntity(List<CreateDestinationRequestDTO>requestDTOs ,  )
    }
}

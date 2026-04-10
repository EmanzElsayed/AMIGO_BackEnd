using Amigo.Domain.DTO.Price;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amigo.Application.Mapping
{
    public  class PriceMapping : IPriceMapping
    {
        public  List<Price> PricesDTOToEntity(List<CreatePriceRequestDTO> requestDTO, Tour tour,string language)
        {
            Language mappedlanguage = EnumsMapping.ToLanguageEnum(language);
            return
                requestDTO.Select(priceDTO => new Price
            {

                Id = Guid.NewGuid(),
                Tour = tour,
                TourId = tour.Id,
                Cost = priceDTO.Cost,
                Discount = priceDTO.Discount ?? 0,
                UserType = priceDTO.UserType,

                Translations =  new List<PriceTranslation> 
                {
                    new PriceTranslation
                    {
                        Id = Guid.NewGuid(),
                        Language = mappedlanguage,
                        Type = priceDTO.Type
                    }
                }
            }).ToList();


            


            

          
           
        }

       
    }
}

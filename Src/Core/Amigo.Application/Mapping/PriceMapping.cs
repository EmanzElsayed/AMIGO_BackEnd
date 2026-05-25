using Amigo.Domain.DTO.Price;
using Amigo.Application.Helpers;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amigo.Application.Mapping
{
    public static class PriceMapping 
    {
        public static List<Price> PricesDTOToEntity(List<CreatePriceRequestDTO> requestDTO, Tour tour,string language)
        {


                SupportedLanguage mappedlanguage = EnumsMapping.ToLanguageEnum(language);
            return
                requestDTO.Select(priceDTO => new Price
                {

                    Id = Guid.NewGuid(),
                    Tour = tour,
                    TourId = tour.Id,
                    Cost = priceDTO.Cost,
                    Discount = priceDTO.Discount ?? 0,
                    UserType = priceDTO.UserType,
                    IsMainActivityType = string.IsNullOrWhiteSpace(priceDTO.ActivityType)
                    ? null
                    : priceDTO.IsMainActivityType == true,
                    Translations = CreatePriceTranslationsForAllLanguages(priceDTO, mappedlanguage)
                }).ToList();



        }

        private static List<PriceTranslation> CreatePriceTranslationsForAllLanguages(CreatePriceRequestDTO priceDTO, SupportedLanguage sourceLanguage)
        {
            var translations = new List<PriceTranslation>();

            translations.Add(new PriceTranslation
            {
                Id = Guid.NewGuid(),
                Language = sourceLanguage,
                Type = priceDTO.Type,
                ActivityType = string.IsNullOrWhiteSpace(priceDTO.ActivityType) ? null : priceDTO.ActivityType,
            });

            var otherLanguages = TranslationLanguageHelper.GetTargetLanguages(sourceLanguage);
            foreach (var lang in otherLanguages)
            {
                translations.Add(new PriceTranslation
                {
                    Id = Guid.NewGuid(),
                    Language = lang,
                    Type = priceDTO.Type,
                    ActivityType = string.IsNullOrWhiteSpace(priceDTO.ActivityType) ? null : priceDTO.ActivityType,
                });
            }

            return translations;
        }
    }
}

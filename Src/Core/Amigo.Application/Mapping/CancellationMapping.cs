using Amigo.Domain.DTO.Cancellation;
using Amigo.Domain.Enum;
using Amigo.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public static class CancellationMapping 
    {

        public static List<Cancellation> CancellationToEntity(List<CreateCancellationRequestDTO> requestDTO, Tour tour, string language)
        {

            return requestDTO.Select(c => new Cancellation {

                Id = Guid.NewGuid(),
                Tour = tour,
                TourId = tour.Id,
                CancellationBefore = c.CancellationBefore,
                RefundPercentage = c.RefundPercentage,
                CancelationPolicyType = EnumsMapping.ToEnum<CancelationPolicyType>(c.CancelationPolicyType, true)

            }).ToList();  
           
        }

        //public static Cancellation CancellationToEntity(CreateCancellationRequestDTO requestDTO, Tour tour ,string language)
        //{
        //    SupportedLanguage mappedlanguage = EnumsMapping.ToLanguageEnum(language);

        //    var cancellationType = EnumsMapping.ToEnum<CancelationPolicyType>(requestDTO.CancelationPolicyType, true);
        //    return new Cancellation
        //    {
        //        Id = Guid.NewGuid(),
        //        Tour = tour,
        //        TourId = tour.Id,
        //        CancellationBefore = requestDTO.CancellationBefore,
        //        RefundPercentage = requestDTO.RefundPercentage,
        //        CancelationPolicyType = cancellationType,
        //        Translations = string.IsNullOrWhiteSpace(requestDTO.Description) 
        //                        ? new List<CancellationTranslation>()
        //                        : CreateCancellationTranslationsForAllLanguages(requestDTO.Description.Trim(), mappedlanguage)

        //    };
        //}

        private static List<CancellationTranslation> CreateCancellationTranslationsForAllLanguages(string description, SupportedLanguage sourceLanguage)
        {
            var translations = new List<CancellationTranslation>();

            translations.Add(new CancellationTranslation
            {
                Id = Guid.NewGuid(),
                Description = description,
                Language = sourceLanguage
            });

            var otherLanguages = TranslationLanguageHelper.GetTargetLanguages(sourceLanguage);
            foreach (var lang in otherLanguages)
            {
                translations.Add(new CancellationTranslation
                {
                    Id = Guid.NewGuid(),
                    Description = description,
                    Language = lang
                });
            }

            return translations;
        }
    }
}

using Amigo.Domain.DTO.Cancellation;
using Amigo.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public class CancellationMapping : ICancellationMapping
    {
        public Cancellation CancellationToEntity(CreateCancellationRequestDTO requestDTO, Tour tour ,string language)
        {
            Language mappedlanguage = EnumsMapping.ToLanguageEnum(language);

            var cancellationType = EnumsMapping.ToEnum<CancelationPolicyType>(requestDTO.CancelationPolicyType, true);
            return new Cancellation
            {
                Id = Guid.NewGuid(),
                Tour = tour,
                TourId = tour.Id,
                CancellationBefore = requestDTO.CancellationBefore,
                RefundPercentage = requestDTO.RefundPercentage,
                CancelationPolicyType = cancellationType,
                Translations = string.IsNullOrWhiteSpace(requestDTO.Description) 
                                ? new List<CancellationTranslation>()
                                : new List<CancellationTranslation>() {

                                    new CancellationTranslation
                                    {
                                        Id = Guid.NewGuid(),
                                        Description = requestDTO.Description.Trim(),
                                        Language = mappedlanguage
                                    }

                                }

            };
        }
        //public CancellationTranslation CancellationTranslationToEntity(CreateCancellationRequestDTO requestDTO, Cancellation cancellation,string language)
        //{
        //    Language mappedlanguage = EnumsMapping.ToLanguageEnum(language);
        //    return new CancellationTranslation
        //    {
        //        Id = Guid.NewGuid(),
        //       Cancellation = cancellation,
        //       CancellationId = cancellation.Id,
        //       Language = mappedlanguage,
        //       Description = requestDTO.Description


        //    };
        //}
    }
}

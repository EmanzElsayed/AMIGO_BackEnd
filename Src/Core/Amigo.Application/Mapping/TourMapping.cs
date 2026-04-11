using Amigo.Domain.DTO.Tour;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public class TourMapping : ITourMapping
    {
        public Tour TourToEntity(CreateTourRequestDTO requestDTO , Destination destination)
        {
            //Language? guideLanguage = null;
            //if (requestDTO.GuideLanguage is not null)
            //{ 
            //    guideLanguage = EnumsMapping.ToLanguageEnum(requestDTO.GuideLanguage);

            //}
            Currency currency = EnumsMapping.ToEnum<Currency>(requestDTO.Currency, false);
            return new Tour()
            {
                Id = Guid.NewGuid(),
                GuideLanguage = requestDTO.GuideLanguage,
                MeetingPoint = requestDTO.MeetingPoint,
                Duration = requestDTO.Duration,
                IsPitsAllowed = requestDTO.IsPitsAllowed,
                IsWheelchairAvailable = requestDTO.IsWheelchairAvailable,
                DestinationId = destination.Id,
                Destination = destination,
                UserType = requestDTO.UserType,
                CurrencyCode = currency

            };
        }

        public TourTranslation TourTranslationToEntity(CreateTourRequestDTO requestDTO, Tour tour)
        {
            Language language = EnumsMapping.ToLanguageEnum(requestDTO.Language);
            return new TourTranslation()
            {
                Id = Guid.NewGuid(),
                Title = requestDTO.Title,
                Description = requestDTO.Description,
                Language = language,
                TourId = tour.Id,
                Tour = tour
            };
        }
    }
}

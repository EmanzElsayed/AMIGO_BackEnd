using Amigo.Domain.DTO.Tour;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public static class TourMapping 
    {
        public static Tour TourToEntity(CreateTourRequestDTO requestDTO , Destination destination)
        {
            
            CurrencyCode currency = EnumsMapping.ToEnum<CurrencyCode>(requestDTO.Currency, false);
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

        public static TourTranslation TourTranslationToEntity(CreateTourRequestDTO requestDTO, Tour tour)
        {
            SupportedLanguage language = EnumsMapping.ToLanguageEnum(requestDTO.Language);
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

        public static void UpdateTour(
                UpdateTourRequestDTO requestDTO,
                Tour tour,
                TourTranslation? translation,
                SupportedLanguage? language)
        {
            // basic fields
            if (requestDTO.Duration is not null)
                tour.Duration = requestDTO.Duration.Value;

            if (requestDTO.MeetingPoint is not null)
                tour.MeetingPoint = requestDTO.MeetingPoint;

            if (requestDTO.GuideLanguage is not null)
                tour.GuideLanguage = requestDTO.GuideLanguage;

            if (requestDTO.UserType is not null)
                tour.UserType = requestDTO.UserType.Value;

            if (requestDTO.Currency is not null)
                tour.CurrencyCode = EnumsMapping.ToEnum<CurrencyCode>(requestDTO.Currency, false);

            if (requestDTO.IsPitsAllowed is not null)
                tour.IsPitsAllowed = requestDTO.IsPitsAllowed.Value;

            if (requestDTO.IsWheelchairAvailable is not null)
                tour.IsWheelchairAvailable = requestDTO.IsWheelchairAvailable.Value;

            if ((requestDTO.Title is not null || requestDTO.Description is not null)
                && language is not null)
            {
                if (translation is null)
                {
                    //  add new language
                    tour.Translations.Add(new TourTranslation
                    {

                        Language = language.Value,
                        Title = requestDTO.Title,
                        Description = requestDTO.Description,
                        TourId = tour.Id
                    });
                }
                else
                {
                    // update existing language
                    if (requestDTO.Title is not null)
                        translation.Title = requestDTO.Title;

                    if (requestDTO.Description is not null)
                        translation.Description = requestDTO.Description;
                }
            }
        }
    }
}

using Amigo.Application.Helpers;
using Amigo.Domain.DTO.BlackoutDate;
using Amigo.Domain.DTO.BlackoutWeekDays;
using Amigo.Domain.DTO.Tour;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public static class TourMapping 
    {

        public static List<BlackoutDate> BlackoutDatesToEntity(List<CreateBlackoutDateRequestDTO> requestDTO, Tour tour)
        {
            return requestDTO.Select(d => new BlackoutDate
            {
                Id = Guid.NewGuid(),
                Tour = tour,
                TourId = tour.Id,
                Date = d.Date
            }).ToList();
        }

        public static List<BlackoutWeekDay> BlackoutWeekDaysToEntity(List<CreateBlackoutWeekDaysRequestDTO> requestDTO, Tour tour)
        {
            return requestDTO.Select(d => new BlackoutWeekDay
            {
                Id = Guid.NewGuid(),
                Tour = tour,
                TourId = tour.Id,
                DayOfWeek = d.DayOfWeek,
            }).ToList();
        }
        public static Tour TourToEntity(CreateTourRequestDTO requestDTO , Destination destination,UserType userType)
        {

            CurrencyCode currency = EnumsMapping.ToEnum<CurrencyCode>(requestDTO.Currency, false);

            SupportedLanguage? guideLanguage = requestDTO.GuideLanguage is null ? null
                                            : requestDTO.GuideLanguage.Aggregate(
                                            SupportedLanguage.None,
                                            (acc, lang) => acc | lang);
            return new Tour()
            {
                Id = Guid.NewGuid(),
                GuideLanguage = guideLanguage,
                MeetingPoint = requestDTO.MeetingPoint,
                Duration = requestDTO.Duration,
                IsPitsAllowed = requestDTO.IsPitsAllowed,
                IsWheelchairAvailable = requestDTO.IsWheelchairAvailable,
                DestinationId = destination.Id,
                Destination = destination,
                UserType = userType,
                CurrencyCode = currency,
                IsFullTime = requestDTO.IsFullTime ?? false,

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

        public static List<TourTranslation> TourTranslationsToEntity(CreateTourRequestDTO requestDTO, Tour tour)
        {
            SupportedLanguage sourceLanguage = EnumsMapping.ToLanguageEnum(requestDTO.Language);
            var translations = new List<TourTranslation>();

            translations.Add(new TourTranslation
            {
                Id = Guid.NewGuid(),
                Title = requestDTO.Title,
                Description = requestDTO.Description,
                Language = sourceLanguage,
                TourId = tour.Id,
                Tour = tour
            });

            var otherLanguages = TranslationLanguageHelper.GetTargetLanguages(sourceLanguage);
            foreach (var lang in otherLanguages)
            {
                translations.Add(new TourTranslation
                {
                    Id = Guid.NewGuid(),
                    Title = requestDTO.Title,
                    Description = requestDTO.Description,
                    Language = lang,
                    TourId = tour.Id,
                    Tour = tour
                });
            }

            return translations;
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
            {
                tour.GuideLanguage = requestDTO.GuideLanguage is null ? null
                                            : requestDTO.GuideLanguage.Aggregate(
                                            SupportedLanguage.None,
                                            (acc, lang) => acc | lang);
            }

            if (requestDTO.UserType is not null)
            {
                tour.UserType = requestDTO.UserType is null ? UserType.Public
                                           : requestDTO.UserType.Aggregate(
                                           UserType.None,
                                           (acc, type) => acc | type);
            }


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

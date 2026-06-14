using Amigo.Application.Helpers;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.QueryParams;
using Stripe;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Amigo.Application.Validation.Common.Specifications;

public static class TourCatalogCriteria
{
    public static Expression<Func<Tour, bool>> Build(
        Guid destinationId,
        GetUserToursQuery q,
        SupportedLanguage? translationLanguage,
        SupportedLanguage? effectiveGuideLanguage,
        //CurrencyCode? currencyFilter,
        CountryCode? destinationCountryFilter,
        UserType? userTypeFilter,
        DateOnly? availabilityDate,
        decimal? maxPrice,
        decimal? minPrice)
    {
        DayOfWeek? day = availabilityDate is null ? null : availabilityDate.Value.DayOfWeek;
        return t =>
            t.DestinationId == destinationId
            && !t.IsDeleted

            && (!destinationCountryFilter.HasValue
                || t.Destination.CountryInfo.CountryCode == destinationCountryFilter.Value)

            //&& (!currencyFilter.HasValue
            //    || t.CurrencyCode == currencyFilter.Value)

            && (string.IsNullOrWhiteSpace(q.DestinationName)
                || t.Destination.Translations.Any(dt =>
                    (translationLanguage == null || dt.Language == translationLanguage)

                    && dt.Name.Contains(q.DestinationName!)))

            && (string.IsNullOrWhiteSpace(q.TourTitle)
                || t.Translations.Any(tt =>
                    (translationLanguage == null || tt.Language == translationLanguage)
                    && tt.Title.Contains(q.TourTitle!)))


            && (!q.MinDurationHours.HasValue
                || t.Duration.TotalHours >= q.MinDurationHours.Value)


            && (!q.MaxDurationHours.HasValue
                || t.Duration.TotalHours <= q.MaxDurationHours.Value)

            && (!minPrice.HasValue
                || t.Prices.Any(p =>
                    !p.IsDeleted
                    && p.Cost * (1 - p.Discount / 100m) >= minPrice!.Value)) 

            && (!maxPrice.HasValue
                || t.Prices.Any(p =>
                    !p.IsDeleted
                    && p.Cost * (1 - p.Discount / 100m) <= maxPrice!.Value))


            && (!availabilityDate.HasValue
                    || t.BlackoutDates == null
                    || !t.BlackoutDates.Any(d => d.Date == availabilityDate.Value))

         && (!day.HasValue
                            || t.BlackoutWeekDays == null
                            || !t.BlackoutWeekDays.Any(d => d.DayOfWeek == day.Value))

            && (!effectiveGuideLanguage.HasValue
                || (t.GuideLanguage.HasValue && ((t.GuideLanguage.Value & effectiveGuideLanguage.Value) == effectiveGuideLanguage.Value)))

            && (q.IsPitsAllowed != true || t.IsPitsAllowed)

            && (q.IsWheelchairAvailable != true || t.IsWheelchairAvailable)
            && (!userTypeFilter.HasValue
                || (t.UserType & userTypeFilter.Value) == userTypeFilter.Value)

            && (string.IsNullOrWhiteSpace(q.Category)
                        || t.TourInclusions.Any(i =>
                            i.Translations.Any(tr =>
                                tr.Language == (translationLanguage ?? SupportedLanguage.en)
                                && tr.Text.ToLower().Trim().Contains(q.Category!.ToLower().Trim())
                            )
                        )
                    )

           

            && (q.FreeCancellation != true
                || (t.Cancellations == null) 
                || (t.Cancellations.Any(c => !c.IsDeleted && c.CancelationPolicyType == CancelationPolicyType.Free))
                    
            )

            && (q.HotelPickup != true
                || t.TourInclusions.Any(i =>
                    i.Translations.Any(tr =>
                        (translationLanguage == null || tr.Language == translationLanguage)
                        && tr.Text.ToLower().Trim().Contains("pickup")
                    )
                )
            )

          

            && (q.OnlyInUserLanguage != true
                || t.Translations.Any(tr =>
                    tr.Language == translationLanguage
                )
            );
    }

    public static Expression<Func<Tour, bool>> BuildAdminTourCatalog(
             Guid? destinationId,
             string? tourTitle,
             string?language,
             bool filterActiveOnly = false
        )
    {
        SupportedLanguage translationLanguage = SupportedLanguage.en;
        if (!string.IsNullOrEmpty(language))
        {
            translationLanguage = EnumsMapping.ToLanguageEnum(language);
        }

        return t => !t.IsDeleted
                &&

                 (destinationId == null || t.DestinationId == destinationId)

                &&
                 (
                    string.IsNullOrWhiteSpace(tourTitle)

                    ||

                (
                    t.Translations.Any(x => x.Language == translationLanguage)
                    &&
                    t.Translations.Any(x =>
                        x.Language == translationLanguage &&
                        SlugHelper.MatchesName(tourTitle, x.Title))
                )

                ||

                (
                    !t.Translations.Any(x => x.Language == translationLanguage)
                    &&
                    t.Translations.Any(x =>
                        SlugHelper.MatchesName(tourTitle, x.Title))
                )
            )


              && (
                t.Destination.Translations.Any(x =>
                    x.Language == translationLanguage)


             )
             &&
            t.Prices.Any(p => p.Translations.Any(t => t.Language == translationLanguage))
           /* &&
            (!filterActiveOnly || t.AvailableTimes.Any(at => (at.EndDate ?? at.StartDate) >= DateOnly.FromDateTime(DateTime.UtcNow)))
*/            ;





    }
}

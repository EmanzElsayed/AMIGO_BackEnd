using Amigo.Application.Helpers;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.QueryParams;
using System.Linq.Expressions;

namespace Amigo.Application.Validation.Common.Specifications;

public static class TourCatalogCriteria
{
    public static Expression<Func<Tour, bool>> Build(
        Guid destinationId,
        GetUserToursQuery q,
        Language? translationLanguage,
        Language? effectiveGuideLanguage,
        CurrencyCode? currencyFilter,
        CountryCode? destinationCountryFilter,
        UserType? userTypeFilter,
        DateOnly? availabilityDate)
    {
        return t =>
            t.DestinationId == destinationId
            && !t.IsDeleted

            && (!destinationCountryFilter.HasValue
                || t.Destination.CountryCode == destinationCountryFilter.Value)

            && (!currencyFilter.HasValue
                || t.CurrencyCode == currencyFilter.Value)

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

            && (!q.MinPrice.HasValue
                || t.Prices.Any(p =>
                    !p.IsDeleted
                    && p.RetailPrice >= q.MinPrice!.Value)) // use retail price

            && (!q.MaxPrice.HasValue
                || t.Prices.Any(p =>
                    !p.IsDeleted
                    && p.RetailPrice <= q.MaxPrice!.Value))//use retail price

            && (!availabilityDate.HasValue
                || t.AvailableTimes.Any(ts =>
                    !ts.IsDeleted
                    && ts.StartDate <= availabilityDate.Value
                    && (ts.EndDate == null || ts.EndDate >= availabilityDate.Value)
                    && ts.AvailableSlots.Any(s =>
                        !s.IsDeleted
                        && s.AvailableTimeStatus == AvailableDateTimeStatus.Available)))


            && (!effectiveGuideLanguage.HasValue
                || t.GuideLanguage == effectiveGuideLanguage.Value)

            && (q.IsPitsAllowed != true || t.IsPitsAllowed)

            && (q.IsWheelchairAvailable != true || t.IsWheelchairAvailable)
            && (!userTypeFilter.HasValue
                || (t.UserType & userTypeFilter.Value) == userTypeFilter.Value)

            && (string.IsNullOrWhiteSpace(q.Category)
                        || t.TourInclusions.Any(i =>
                            i.Translations.Any(tr =>
                                tr.Language == (translationLanguage ?? Language.en)
                                && tr.Text.ToLower().Trim().Contains(q.Category!.ToLower().Trim())
                            )
                        )
                    )

            && (q.FreeCancellation != true
                || (t.Cancellation != null
                    && !t.Cancellation.IsDeleted
                    && t.Cancellation.CancelationPolicyType == CancelationPolicyType.Free)
            )

            && (q.HotelPickup != true
                || t.TourInclusions.Any(i =>
                    i.Translations.Any(tr =>
                        (translationLanguage == null || tr.Language == translationLanguage)
                        && tr.Text.ToLower().Trim().Contains("pickup")
                    )
                )
            )

            && (q.RequireAvailableSlots != true
                || t.AvailableTimes.Any(ts =>
                    !ts.IsDeleted
                    && ts.AvailableSlots.Any(s =>
                        !s.IsDeleted
                        && s.AvailableTimeStatus == AvailableDateTimeStatus.Available
                    )
                )
            );
    }

    public static Expression<Func<Tour, bool>> BuildAdminTourCatalog(
             Guid? destinationId,
             string? tourTitle,
             string?language
             //Language translationLanguage = Language.English



        )
    {
        Language translationLanguage = Language.en;
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
            ;





    }
}

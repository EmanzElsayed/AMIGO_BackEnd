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
        Currency? currencyFilter,
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
                    && p.Cost * (1 - p.Discount / 100m) >= q.MinPrice!.Value))
            && (!q.MaxPrice.HasValue
                || t.Prices.Any(p =>
                    !p.IsDeleted
                    && p.Cost * (1 - p.Discount / 100m) <= q.MaxPrice!.Value))
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
                || t.Included.Any(i =>
                    i.Language == (translationLanguage ?? Language.English)
                    && i.Included == q.Category))
            && (q.FreeCancellation != true
                || (t.Cancellation != null
                    && !t.Cancellation.IsDeleted
                    && t.Cancellation.CancelationPolicyType == CancelationPolicyType.Free))
            && (q.HotelPickup != true
                || t.Included.Any(i =>
                    (translationLanguage == null || i.Language == translationLanguage)
                    && i.Included.ToLower().Contains("pickup")))
            && (q.RequireAvailableSlots != true
                || t.AvailableTimes.Any(ts =>
                    !ts.IsDeleted
                    && ts.AvailableSlots.Any(s =>
                        !s.IsDeleted
                        && s.AvailableTimeStatus == AvailableDateTimeStatus.Available)));
    }
}

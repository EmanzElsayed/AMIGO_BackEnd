using Amigo.Application.Helpers;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.DTOs.Tour;

namespace Amigo.Application.Mapping;

public static class UserTourCatalogMapper
{
    public static UserTourDetailDto ToDetail(
        Tour tour,
        Language listingLanguage,
        IReadOnlyList<Price> pricesWithTranslations,
        UserType? effectiveUserType,
        IReadOnlyList<TourSchedule> schedules,
        IReadOnlyList<Review> reviews,
        IReadOnlyList<ReviewTranslation> reviewTranslations,
        DateOnly todayUtc,
        string? cancellationPolicyDescription)
    {
        var baseItem = ToListItem(tour, listingLanguage, effectiveUserType);
        var tiers = MapPriceTiers(pricesWithTranslations, listingLanguage, effectiveUserType);
        var days = MapScheduleDays(schedules, todayUtc);
        var recent = MapRecentReviews(reviews, reviewTranslations);
        var travelerPhotos = MapTravelerPhotos(reviews);
        var imageUrls = tour.Images
            .Where(i => !i.IsDeleted && !string.IsNullOrWhiteSpace(i.ImageUrl))
            .OrderBy(i => i.Id)
            .Select(i => i.ImageUrl.Trim())
            .Distinct()
            .ToList();

        return new UserTourDetailDto(
            TourId: baseItem.TourId,
            Title: baseItem.Title,
            Description: baseItem.Description,
            HeroImageUrl: baseItem.HeroImageUrl,
            ImageUrls: imageUrls,
            AverageRating: baseItem.AverageRating,
            ReviewCount: baseItem.ReviewCount,
            FreeCancellation: baseItem.FreeCancellation,
            IsWheelchairAvailable: baseItem.IsWheelchairAvailable,
            IsPitsAllowed: baseItem.IsPitsAllowed,
            OriginalPrice: baseItem.OriginalPrice,
            FromPrice: baseItem.FromPrice,
            DiscountPercent: baseItem.DiscountPercent,
            DurationDisplay: baseItem.DurationDisplay,
            GuideLanguage: baseItem.GuideLanguage,
            TourSlug: baseItem.TourSlug,
            CurrencyCode: tour.CurrencyCode.ToString(),
            PriceTiers: tiers,
            ScheduleDays: days,
            RecentReviews: recent,
            TravelerPhotos: travelerPhotos,
            MeetingPoint: string.IsNullOrWhiteSpace(tour.MeetingPoint) ? null : tour.MeetingPoint.Trim(),
            Included: MapIncludedLines(tour, listingLanguage),
            NotIncluded: MapNotIncludedLines(tour, listingLanguage),
            CancellationPolicyDescription: string.IsNullOrWhiteSpace(cancellationPolicyDescription)
                ? null
                : cancellationPolicyDescription.Trim());
    }

    private static IReadOnlyList<string> MapIncludedLines(Tour tour, Language lang)
    {
        var inclusions = tour.TourInclusions
            .Where(i => !i.IsDeleted && i.IsIncluded) 
            .ToList();

        if (inclusions.Count == 0)
            return [];

        var primary = inclusions
            .SelectMany(i => i.Translations)
            .Where(t => t.Language == lang && !string.IsNullOrWhiteSpace(t.Text))
            .Select(t => t.Text.Trim())
            .ToList();

        if (primary.Count > 0)
            return primary;

        return inclusions
            .SelectMany(i => i.Translations)
            .Where(t => !string.IsNullOrWhiteSpace(t.Text))
            .Select(t => t.Text.Trim())
            .Distinct()
            .ToList();
    }

    private static IReadOnlyList<string> MapNotIncludedLines(Tour tour, Language lang)
    {
        var inclusions = tour.TourInclusions
           .Where(i => !i.IsDeleted && !i.IsIncluded)
           .ToList();

        if (inclusions.Count == 0)
            return [];

        var primary = inclusions
            .SelectMany(i => i.Translations)
            .Where(t => t.Language == lang && !string.IsNullOrWhiteSpace(t.Text))
            .Select(t => t.Text.Trim())
            .ToList();

        if (primary.Count > 0)
            return primary;

        return inclusions
            .SelectMany(i => i.Translations)
            .Where(t => !string.IsNullOrWhiteSpace(t.Text))
            .Select(t => t.Text.Trim())
            .Distinct()
            .ToList();
    }

    private static IReadOnlyList<UserTourPriceTierDto> MapPriceTiers(
        IReadOnlyList<Price> prices,
        Language listingLanguage,
        UserType? effectiveUserType)
    {
        var list = new List<UserTourPriceTierDto>();
        var allowedUserType = NormalizeEffectiveUserType(effectiveUserType);
        foreach (var p in prices
            .Where(x => !x.IsDeleted && (x.UserType & allowedUserType) == allowedUserType)
            .OrderBy(x => x.RetailPrice))
        {
            var tr = p.Translations.FirstOrDefault(x => x.Language == listingLanguage)
                     ?? p.Translations.FirstOrDefault();
            var label = tr?.Type ?? "Traveler";
            var retail = p.RetailPrice;
            var isFree = retail <= 0;
            var group = p.UserType.HasFlag(UserType.VIP) ? "VIP" : "Public";
            list.Add(new UserTourPriceTierDto(p.Id, label, retail, isFree, group));
        }

        return list;
    }

    private static IReadOnlyList<UserTourScheduleDayDto> MapScheduleDays(
        IReadOnlyList<TourSchedule> schedules,
        DateOnly todayUtc)
    {
        var byDay = new Dictionary<DateOnly, List<UserTourSlotDto>>();

        foreach (var schedule in schedules)
        {
            if (schedule.AvailableDateStatus != AvailableDateTimeStatus.Available)
                continue;

            var rangeEnd = schedule.EndDate ?? schedule.StartDate;
            if (rangeEnd < schedule.StartDate)
                continue;

            var cappedEnd = rangeEnd;
            var maxByStart = schedule.StartDate.AddDays(400);
            if (cappedEnd > maxByStart)
                cappedEnd = maxByStart;

            for (var day = schedule.StartDate; day <= cappedEnd; day = day.AddDays(1))
            {
                if (day < todayUtc)
                    continue;

                foreach (var slot in schedule.AvailableSlots.Where(s => !s.IsDeleted))
                {
                    if (slot.AvailableTimeStatus != AvailableDateTimeStatus.Available)
                        continue;

                    var dto = new UserTourSlotDto(
                        slot.Id,
                        slot.StartTime.ToString("HH:mm"),
                        slot.EndTime?.ToString("HH:mm"),
                        true);

                    if (!byDay.TryGetValue(day, out var list))
                    {
                        list = [];
                        byDay[day] = list;
                    }

                    if (list.All(x => x.SlotId != dto.SlotId))
                        list.Add(dto);
                }
            }
        }

        return byDay
            .OrderBy(x => x.Key)
            .Select(x => new UserTourScheduleDayDto(
                x.Key.ToString("yyyy-MM-dd"),
                x.Value.OrderBy(s => s.StartTime).ToList()))
            .ToList();
    }

    private static IReadOnlyList<UserTourReviewItemDto> MapRecentReviews(
        IReadOnlyList<Review> reviews,
        IReadOnlyList<ReviewTranslation> translations)
    {
        return reviews
            .OrderByDescending(r => r.Date)
            .Take(24)
            .Select(r =>
            {
                var tr = translations.FirstOrDefault(t => t.ReviewId == r.Id);
                var comment = tr?.Comment;
                var author = r.User?.UserName ?? r.User?.Email;
                var imageUrls = r.Images
                    .Where(i => !i.IsDeleted && !string.IsNullOrWhiteSpace(i.Image))
                    .OrderBy(i => i.Id)
                    .Select(i => i.Image.Trim())
                    .Distinct()
                    .ToList();
                return new UserTourReviewItemDto(
                    r.Rate,
                    comment,
                    string.IsNullOrWhiteSpace(author) ? null : author,
                    r.Date.ToString("yyyy-MM-dd"),
                    imageUrls);
            })
            .ToList();
    }

    private static IReadOnlyList<UserTourTravelerPhotoDto> MapTravelerPhotos(
        IReadOnlyList<Review> reviews)
    {
        var rows = reviews
            .Where(r => !r.IsDeleted)
            .OrderByDescending(r => r.Date)
            .SelectMany(r =>
            {
                var author = r.User?.UserName ?? r.User?.Email;
                return r.Images
                    .Where(i => !i.IsDeleted && !string.IsNullOrWhiteSpace(i.Image))
                    .Select(i => new UserTourTravelerPhotoDto(
                        i.Image.Trim(),
                        r.Date.ToString("yyyy-MM-dd"),
                        string.IsNullOrWhiteSpace(author) ? null : author));
            })
            .GroupBy(x => x.ImageUrl, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .Take(120)
            .ToList();

        return rows;
    }

    public static UserTourListItemDto ToListItem(Tour tour, Language listingLanguage)
        => ToListItem(tour, listingLanguage, null);

    public static UserTourListItemDto ToListItem(Tour tour, Language listingLanguage, UserType? effectiveUserType)
    {
        var tr = tour.Translations
            .FirstOrDefault(x => x.Language == listingLanguage)
            ?? tour.Translations.FirstOrDefault();

        var title = tr?.Title ?? string.Empty;
        var description = tr?.Description;

        var hero = tour.Images
            .Where(i => !i.IsDeleted)
            .OrderBy(i => i.Id)
            .FirstOrDefault();

        var reviews = tour.Reviews.Where(r => !r.IsDeleted).ToList();
        decimal? avg = reviews.Count == 0 ? null : reviews.Average(r => r.Rate);

        var free = tour.Cancellation != null
                   && !tour.Cancellation.IsDeleted
                   && tour.Cancellation.CancelationPolicyType == CancelationPolicyType.Free;

        var allowedUserType = NormalizeEffectiveUserType(effectiveUserType);
        var prices = tour.Prices
            .Where(p => !p.IsDeleted && (p.UserType & allowedUserType) == allowedUserType)
            .ToList();
        decimal? minRetail = prices.Count == 0
            ? null
            : prices.Min(p => p.Cost * (1 - p.Discount / 100m));

        var currency = tour.CurrencyCode.ToString();
        string? fromDisplay = minRetail.HasValue ? $"{currency} {minRetail.Value:0.##}" : null;

        string? originalDisplay = null;
        int? discountPct = null;
        if (prices.Count > 0 && minRetail.HasValue)
        {
            var minListPrice = prices.Min(p => p.Cost);
            if (tour.Discount is > 0)
            {
                discountPct = (int)Math.Round(tour.Discount.Value);
                originalDisplay = $"{currency} {minListPrice:0.##}";
            }
            else if (minListPrice > minRetail.Value + 0.0001m)
            {
                var inferredPct = (int)Math.Round((1 - minRetail.Value / minListPrice) * 100m);
                if (inferredPct > 0)
                {
                    discountPct = inferredPct;
                    originalDisplay = $"{currency} {minListPrice:0.##}";
                }
            }
        }

        var dur = tour.Duration;
        var durationDisplay = dur.TotalHours >= 24
            ? $"{(int)dur.TotalDays}d {dur.Hours}h"
            : dur.TotalHours >= 1
                ? $"{(int)dur.TotalHours}h {dur.Minutes}m"
                : $"{dur.Minutes}m";

        var guide = tour.GuideLanguage?.ToString();

        return new UserTourListItemDto(
            TourId: tour.Id,
            Title: title,
            Description: description,
            HeroImageUrl: hero?.ImageUrl,
            AverageRating: avg,
            ReviewCount: reviews.Count,
            FreeCancellation: free,
            IsWheelchairAvailable: tour.IsWheelchairAvailable,
            IsPitsAllowed: tour.IsPitsAllowed,
            OriginalPrice: originalDisplay,
            FromPrice: fromDisplay,
            DiscountPercent: discountPct,
            DurationDisplay: durationDisplay,
            GuideLanguage: guide,
            TourSlug: SlugHelper.ToUrlSlug(title)
        );
    }

    private static UserType NormalizeEffectiveUserType(UserType? effectiveUserType)
        => effectiveUserType == UserType.VIP ? UserType.VIP : UserType.Public;
}

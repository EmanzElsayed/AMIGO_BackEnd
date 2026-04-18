using Amigo.Application.Abstraction.Services;
using Amigo.Application.Mapping;
using Amigo.Application.Specifications.DestinationSpecification.User;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.DTOs.Destination;
using Amigo.SharedKernal.QueryParams;
using Microsoft.EntityFrameworkCore;

namespace Amigo.Persistence.Services;

public class TopDestinationsReader(AmigoDbContext _db) : ITopDestinationsReader
{
    public async Task<IReadOnlyList<TopDestinationSummaryResponseDTO>> GetTopAsync(
        GetTopDestinationsQuery query,
        CancellationToken cancellationToken = default)
    {
        CurrencyCode? currencyFilter = null;
        if (!string.IsNullOrWhiteSpace(query.Currency)
            && Enum.TryParse<CurrencyCode>(query.Currency, true, out var c)
            && c != CurrencyCode.None)
        {
            currencyFilter = c;
        }

        var preferredLanguage = string.IsNullOrWhiteSpace(query.Language)
            ? Language.English
            : EnumsMapping.ToLanguageEnum(query.Language!);

        if (preferredLanguage == Language.None)
            preferredLanguage = Language.English;

        var rankingSpec = new TopDestinationsRankingSpecification();
        var eligibleDestinations = SpeceficationEvaluator.CreateQuery(
            _db.Destinations.AsNoTracking(),
            rankingSpec);

        var statsQuery =
            from d in eligibleDestinations
            let activeTours = d.Tours.Where(t => !t.IsDeleted)
            select new
            {
                d.Id,
                d.ImageUrl,
                CountryCode = d.CountryCode.ToString(),
                ActivityCount = activeTours.Count(),
                ReviewCount = activeTours
                    .SelectMany(t => t.Reviews.Where(r => !r.IsDeleted))
                    .Count(),
                AverageRating = activeTours
                    .SelectMany(t => t.Reviews.Where(r => !r.IsDeleted))
                    .Average(r => (double?)r.Rate),
                TravelerCount = 0/*_db.Bookings
                    .Where(b =>
                        !b.IsDeleted
                        && b.AvailableSlots.TourSchedule.Tour.DestinationId == d.Id
                        && !b.AvailableSlots.TourSchedule.Tour.IsDeleted)
                    .SelectMany(b => b.PeopleBookings.Where(pb => !pb.IsDeleted))
                    .Sum(pb => (int?)pb.NoOfPeopleBooking) ?? 0*/,
                MinFromPrice = currencyFilter == null
                    ? null
                    : activeTours
                        .Where(t => t.CurrencyCode == currencyFilter.Value)
                        .SelectMany(t => t.Prices.Where(p => !p.IsDeleted))
                        .Min(p => (decimal?)(p.Cost * (1 - p.Discount / 100m))),
                MinFromPriceCurrency = currencyFilter == null
                    ? null
                    : currencyFilter.Value.ToString()
            };

        var stats = await statsQuery.ToListAsync(cancellationToken);

        var ranked = stats
            .OrderByDescending(x => x.TravelerCount)
            .ThenByDescending(x => x.ReviewCount)
            .ThenByDescending(x => x.ActivityCount)
            .Take(query.Take)
            .ToList();

        var ids = ranked.Select(x => x.Id).ToList();
        if (ids.Count == 0)
            return Array.Empty<TopDestinationSummaryResponseDTO>();

        var translationSpec = new DestinationTranslationsByDestinationIdsSpecification(ids);
        var translations = await SpeceficationEvaluator
            .CreateQuery(_db.DestinationTranslations.AsNoTracking(), translationSpec)
            .Select(t => new { t.DestinationId, t.Language, t.Name })
            .ToListAsync(cancellationToken);

        string ResolveName(Guid destinationId)
        {
            var list = translations.Where(t => t.DestinationId == destinationId).ToList();
            var exact = list.FirstOrDefault(t => t.Language == preferredLanguage)?.Name;
            if (!string.IsNullOrEmpty(exact))
                return exact!;

            var english = list.FirstOrDefault(t => t.Language == Language.English)?.Name;
            if (!string.IsNullOrEmpty(english))
                return english!;

            return list.OrderBy(t => t.Language).FirstOrDefault()?.Name ?? "";
        }

        return ranked
            .Select(x => new TopDestinationSummaryResponseDTO(
                DestinationId: x.Id,
                Name: ResolveName(x.Id),
                CountryCode: x.CountryCode,
                ImageUrl: x.ImageUrl,
                ActivityCount: x.ActivityCount,
                ReviewCount: x.ReviewCount,
                TravelerCount: x.TravelerCount,
                AverageRating: x.AverageRating

            ))
            .ToList();
    }
}

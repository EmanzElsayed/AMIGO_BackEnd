using Amigo.Application.Abstraction.Services;
using Amigo.Application.Mapping;
using Amigo.Application.Specifications.DestinationSpecification.User;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.DTOs.Destination;
using Amigo.SharedKernal.QueryParams;
using Amigo.SharedKernal.DTOs.Results;
using Microsoft.EntityFrameworkCore;

namespace Amigo.Persistence.Services;

public class TopDestinationsReader(AmigoDbContext _db) : ITopDestinationsReader
{
    public async Task<PaginatedResponse<TopDestinationSummaryResponseDTO>> GetTopAsync(
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
            ? Language.en
            : EnumsMapping.ToLanguageEnum(query.Language!);

        if (preferredLanguage == Language.None)
            preferredLanguage = Language.en;

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
                TravelerCount = 0,
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
            .OrderByDescending(x => x.ActivityCount)
            .ToList();

        var totalItems = ranked.Count;

        var pagedItems = ranked
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        var ids = pagedItems.Select(x => x.Id).ToList();
        if (ids.Count == 0)
        {
            return new PaginatedResponse<TopDestinationSummaryResponseDTO>
            {
                Data = Array.Empty<TopDestinationSummaryResponseDTO>(),
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalItems = totalItems
            };
        }

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

            var english = list.FirstOrDefault(t => t.Language == Language.en)?.Name;
            if (!string.IsNullOrEmpty(english))
                return english!;

            return list.OrderBy(t => t.Language).FirstOrDefault()?.Name ?? "";
        }

        var resultData = pagedItems
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

        return new PaginatedResponse<TopDestinationSummaryResponseDTO>
        {
            Data = resultData,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }
}

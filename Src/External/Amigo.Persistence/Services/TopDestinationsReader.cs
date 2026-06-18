using Amigo.Application.Abstraction.Services;
using Amigo.Application.Helpers;
using Amigo.Application.Mapping;
using Amigo.Application.Specifications.DestinationSpecification.User;
using Amigo.Domain.Abstraction;
using Amigo.Domain.Abstraction.Repositories;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.DTOs.Destination;
using Amigo.SharedKernal.DTOs.Results;
using Amigo.SharedKernal.QueryParams;
using Microsoft.EntityFrameworkCore;
using PayPalCheckoutSdk.Orders;

namespace Amigo.Persistence.Services;

public class TopDestinationsReader(AmigoDbContext _db,ICurrentUserService _currentUserService,IUnitOfWork _unitOfWork) 
    : ITopDestinationsReader
{
    public async Task<PaginatedResponse<TopDestinationSummaryResponseDTO>> GetTopAsync(
        GetTopDestinationsQuery query,
        string? userType,
        CancellationToken cancellationToken = default)
    {
        CurrencyCode? currencyFilter = _currentUserService.Currency;

        var preferredLanguage = _currentUserService.Language;
        

        var rankingSpec = new TopDestinationsRankingSpecification();
        var eligibleDestinations = SpeceficationEvaluator.CreateQuery(
            _db.Destinations.AsNoTracking(),
            rankingSpec);
        var tourIds = await eligibleDestinations
                .SelectMany(d => d.Tours)
                .Where(t => !t.IsDeleted)
                .Select(t => t.Id)
                .ToListAsync(cancellationToken);
        var travelersCount = await _unitOfWork.PriceRepo.GetTravelersCount(tourIds);
        var effectiveUserType = ParseUserType(userType) ?? UserType.Public;

        var statsQuery =
            from d in eligibleDestinations
            let activeTours = d.Tours.Where(t => !t.IsDeleted && (t.UserType & effectiveUserType) == effectiveUserType)
            select new
            {
                d.Id,
                d.ImageUrl,
                CountryCode = d.CountryInfo.CountryCode.ToString(),
                ActivityCount = activeTours.Count(),
                ReviewCount = activeTours
                    .SelectMany(t => t.Reviews.Where(r => !r.IsDeleted))
                    .Count() + Constants.ReviewCount,

                AverageRating =  activeTours
                    .SelectMany(t => t.Reviews.Where(r => !r.IsDeleted))
                    .Average(r => (double?)r.Rate) ?? (double) Constants.AverageReviewRating,

                TravelerCount = travelersCount + Constants.TravelersCount,

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

            var english = list.FirstOrDefault(t => t.Language == SupportedLanguage.en)?.Name;
            if (!string.IsNullOrEmpty(english))
                return english!;

            return list.OrderBy(t => t.Language).FirstOrDefault()?.Name ?? "";
        }

        var resultData = pagedItems
            .Select( x =>
                
                    new TopDestinationSummaryResponseDTO(
                    DestinationId: x.Id,
                    Name: ResolveName(x.Id),
                    CountryCode: x.CountryCode,
                    ImageUrl: x.ImageUrl,
                    ActivityCount: x.ActivityCount,
                    ReviewCount: x.ReviewCount,
                    TravelerCount: x.TravelerCount,
                    AverageRating: x.AverageRating
                )
            )
            .ToList();

        return new PaginatedResponse<TopDestinationSummaryResponseDTO>
        {
            Data = resultData,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }
    private static UserType? ParseUserType(string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return null;
        if (s.Equals("VIP", StringComparison.OrdinalIgnoreCase))
            return UserType.VIP;
        if (s.Equals("Standard", StringComparison.OrdinalIgnoreCase)
            || s.Equals("Public", StringComparison.OrdinalIgnoreCase))
            return UserType.Public;
        return null;
    }

}

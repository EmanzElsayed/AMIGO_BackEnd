using Amigo.Application.Abstraction.Services;
using Amigo.Application.Helpers;
using Amigo.Application.Mapping;
using Amigo.Application.Specifications.CountriesInfo;
using Amigo.Application.Specifications.TourSpecification;
using Amigo.Application.Specifications.TourSpecification.User;
using Amigo.Domain.DTO.Price;
using Amigo.Domain.DTO.Tour;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.Domain.Enum;
using System.Globalization;
using Amigo.Domain.Errors.BusinessErrors;
using Amigo.SharedKernal.DTOs.Results;
using Amigo.SharedKernal.DTOs.Tour;
using Amigo.SharedKernal.QueryParams;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Logging;

namespace Amigo.Application.Services;

public class UserTourCatalogService(
            IValidationService _validationService,
            IUnitOfWork _unitOfWork,
            IDestinationSlugResolver _slugResolver,
            ICurrencyRateService _currencyRateService,
            ICurrentUserService _currentUserService,
            Microsoft.Extensions.Logging.ILogger<UserTourCatalogService> _logger)
                : IUserTourCatalogService
{
    public async Task<Result<PaginatedResponse<UserTourListItemDto>>> GetToursAsync(GetUserToursQuery query)
    {
        var validationResult = await _validationService.ValidateAsync(query);
        if (!validationResult.IsSuccess)
            return validationResult;

        _logger.LogInformation("GetToursAsync received raw AvailabilityDate='{AvailabilityDate}' for DestinationId={DestinationId}", query.AvailabilityDate, query.DestinationId);

        var listingLang = _currentUserService.Language;

        SupportedLanguage? effectiveGuide = query.OnlyInUserLanguage == true
            ? listingLang
            : (string.IsNullOrWhiteSpace(query.GuideLanguage)
                ? null
                : EnumsMapping.ToLanguageEnum(query.GuideLanguage!));

        CountryCode? country = null;
        if (!string.IsNullOrWhiteSpace(query.CountryCode)
            && Enum.TryParse<CountryCode>(query.CountryCode, true, out var cc)
            && cc != CountryCode.None)

            country = cc;

        CurrencyCode filteredCurrency = _currentUserService.Currency;
        var rate = await _currencyRateService.GetRateAsync(
            Constants.BaseCurrency,
            filteredCurrency
                    , true);

        var baseRate = await _currencyRateService.GetRateAsync(

            filteredCurrency,
            Constants.BaseCurrency
                    , true);
        var minPrice = query.MinPrice is null ? null : query.MinPrice * baseRate.ValueOrDefault;
        var maxPrice = query.MaxPrice is null ? null : query.MaxPrice * baseRate.ValueOrDefault;

        UserType? userType = ParseUserType(query.UserType);

        DateOnly? availabilityDate = null;
        if (!string.IsNullOrWhiteSpace(query.AvailabilityDate)
            && DateOnly.TryParse(query.AvailabilityDate, out var parsedDate))
        {
            availabilityDate = parsedDate;
        }

        var destId = query.DestinationId;

        var countSpec = new UserTourCatalogFilterSpecification(
            query,
            destId,
            listingLang,
            effectiveGuide,
            //currencyFilter,
            country,
            userType,
            availabilityDate,
            maxPrice,
            minPrice);

        var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
        var totalItems = await tourRepo.GetCountSpecificationAsync(countSpec);
        _logger.LogInformation("GetToursAsync CountSpecification returned {TotalItems} for DestinationId={DestinationId}, AvailabilityDate={AvailabilityDate}", totalItems, destId, availabilityDate.HasValue ? availabilityDate.Value.ToString("yyyy-MM-dd") : "null");

        var listSpec = new UserTourCatalogSpecification(
            query,
            destId,
            listingLang,
            effectiveGuide,
            //currencyFilter,
            country,
            userType,
            availabilityDate,
            applyPaging: true,
            maxPrice,
            minPrice);

        var tours = (await tourRepo.GetAllAsync(listSpec)).ToList();
        _logger.LogInformation("GetToursAsync fetched {Count} tours for DestinationId={DestinationId}. TourIds={TourIds}", tours.Count, destId, tours.Select(t => t.Id).ToList());

        var tourIds = tours
                .Select(x => x.Id)
                .ToList();
        var effectiveUserType = NormalizeEffectiveUserType(userType);
        var prices =
                await _unitOfWork.PriceRepo
                    .GetTourPriceSummariesAsync(tourIds, effectiveUserType);
        var priceMap = prices
            .GroupBy(p => p.TourId)
            .ToDictionary(
                g => g.Key,
                g => new TourPriceSummaryDto
                {
                    TourId = g.Key,
                    MaxRetailPrice = g.Max(x => x.RetailPrice),
                    MaxCostPrice = g.Max(x => x.CostPrice)
                });
        var reviewRows =
             await _unitOfWork.ReviewRepo
                .GetTourReviewSummariesAsync(tourIds);
        var reviewMap = reviewRows
        .GroupBy(x => x.TourId)
        .ToDictionary(
            g => g.Key,
            g => new TourReviewSummaryDto
            {
                TourId = g.Key,
                ReviewCount = g.Count(),
                AverageRating = g.Average(x => x.Rate)
            });
        var cancellationRows =
            await _unitOfWork.CancellationRepo
                .GetFreeCancellationLookupAsync(tourIds);
        var cancellationMap = cancellationRows
            .GroupBy(x => x.TourId)
            .ToDictionary(
                g => g.Key,
                g => g.Any(x => x.IsFree));
        var data = tours.Select(

        t =>
        {

            var tr = t.Translations
              .FirstOrDefault(x => x.Language == listingLang)
              ?? t.Translations.FirstOrDefault();
            var hero = t.Images
                  .Where(i => !i.IsDeleted)
                  .OrderBy(i => i.Id)
                  .FirstOrDefault();

            priceMap.TryGetValue(t.Id, out var price);
            reviewMap.TryGetValue(t.Id, out var review);
            string? originalDisplay = null;
            int? discountPct = null;
            if (price is not null && price.MaxRetailPrice is not null && price.MaxCostPrice is not null && price.MaxCostPrice > price.MaxRetailPrice + 0.0001m)
            {
                var inferredPct = (int)Math.Round((1 - price.MaxRetailPrice.Value / price.MaxCostPrice.Value) * 100m);
                if (inferredPct > 0)
                {
                    discountPct = inferredPct;
                    originalDisplay = $"{filteredCurrency} {Math.Round(price.MaxCostPrice.Value * rate.ValueOrDefault, 2):0.##}";
                }
            }
            var dur = t.Duration;
            var title = tr?.Title ?? string.Empty;
            return
            new UserTourListItemDto(
                        TourId: t.Id,
                        Title: title,
                        Description: tr?.Description,
                        HeroImageUrl: hero?.ImageUrl,
                        AverageRating: review?.AverageRating,
                        ReviewCount: review?.ReviewCount ?? 0,
                          FreeCancellation: cancellationMap.GetValueOrDefault(t.Id),

                        IsWheelchairAvailable: t.IsWheelchairAvailable,
                        IsPitsAllowed: t.IsPitsAllowed,
                        OriginalPrice: originalDisplay,
                        FromPrice: price is null || price.MaxRetailPrice is null ? null :
                        $"{filteredCurrency} {Math.Round(price.MaxRetailPrice.Value * rate.ValueOrDefault, 2):0.##}",
                        DiscountPercent: discountPct,
                        DurationDisplay: dur.TotalHours >= 24
                            ? $"{(int)dur.TotalDays}d {dur.Hours}h"
                            : dur.TotalHours >= 1
                                ? $"{(int)dur.TotalHours}h {dur.Minutes}m"
                                : $"{dur.Minutes}m",
                        GuideLanguage: t.GuideLanguage?.ToString(),
                         TourSlug: SlugHelper.ToUrlSlug(title)
                                    );
        }
            ).ToList();



        var totalPages = query.PageSize <= 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)query.PageSize);

        var response = new PaginatedResponse<UserTourListItemDto>
        {
            Data = data,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
        };

        return Result.Ok(response);
    }

    public async Task<Result<IEnumerable<string>>> GetTourCategoriesAsync(
     Guid destinationId,
     string? language)
    {
        var lang = _currentUserService.Language;

        var spec = new TourIncludedLinesForDestinationSpecification(destinationId);
        var rows = await _unitOfWork.GetRepository<TourInclusion, Guid>().GetAllAsync(spec);

        var preferred = rows
            .SelectMany(x => x.Translations)
            .Where(t => t.Language == lang && !string.IsNullOrWhiteSpace(t.Text))
            .Select(t => t.Text.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var inclusions = await _unitOfWork
            .GetRepository<TourInclusion, Guid>()
            .GetAllAsync(spec);

        var primary = inclusions
            .SelectMany(i => i.Translations)
            .Where(t => t.Language == lang && !string.IsNullOrWhiteSpace(t.Text))
            .Select(t => t.Text.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (primary.Count > 0)
            return Result.Ok<IEnumerable<string>>(primary.OrderBy(x => x));



        var distinct = preferred.Count > 0
            ? preferred
            : rows
                .SelectMany(x => x.Translations)
                .Where(t => !string.IsNullOrWhiteSpace(t.Text))
                .Select(t => t.Text.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

        return Result.Ok<IEnumerable<string>>(distinct);

    }

    public async Task<Result<MaxDurationHoursResponseDto>> GetMaxDurationHoursForDestinationAsync(Guid destinationId)
    {
        if (destinationId == Guid.Empty)
            return Result.Fail<MaxDurationHoursResponseDto>("DestinationId is required.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var spec = new ToursWithFutureSchedulesForDestinationSpecification(destinationId, today);
        var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
        var maxHours = await tourRepo.MaxAsync(spec, t => t.Duration.TotalHours);

        var hours = maxHours is > 0 ? maxHours.Value : 0.0;

        return Result.Ok(new MaxDurationHoursResponseDto { MaxDurationHours = hours });
    }

    public async Task<Result<MaxPriceResponseDto>> GetMaxPriceForDestinationAsync(Guid destinationId)
    {
        if (destinationId == Guid.Empty)
            return Result.Fail<MaxPriceResponseDto>("DestinationId is required.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var spec = new ToursWithFutureSchedulesForDestinationSpecification(destinationId, today);
        var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
        var tours = await tourRepo.GetAllAsync(spec);

        CurrencyCode filteredCurrency = _currentUserService.Currency;
        var rate = await _currencyRateService.GetRateAsync(
                    Constants.BaseCurrency,
                    filteredCurrency, true);

        var maxPrice = tours
            .SelectMany(t => t.Prices)
            .Where(p => !p.IsDeleted)
            .Max(p => (decimal?)p.RetailPrice) ?? 0m;

        return Result.Ok(new MaxPriceResponseDto { MaxPrice = maxPrice * rate.ValueOrDefault });
    }

    //public async Task<Result<UserTourDetailDto>> GetTourByPublicPathAsync(GetTourByPublicPathQuery query, string? userType, string? currentUserId = null)
    //{
    //    var validationResult = await _validationService.ValidateAsync(query);
    //    if (!validationResult.IsSuccess)
    //        return validationResult;

    //    var destId = await _slugResolver.ResolveDestinationIdAsync(query.DestinationSlug);
    //    if (destId is null)
    //        return Result.Fail(new NotFoundError("Destination not found for this link."));

    //    var listingLang = _currentUserService.Language;
    //    var filteredCurrency = _currentUserService.Currency;

    //    var rate = await _currencyRateService.GetRateAsync(
    //                   Constants.BaseCurrency,
    //                   filteredCurrency, true);

    //    if (!rate.IsSuccess)
    //        return Result.Fail(rate.Errors);


    //    var effectiveUserType = ParseUserType(userType) ?? UserType.Public;

    //    var requestSlug = SlugHelper.ToUrlSlug(query.TourSlug);
    //    var normalizedRequestSlug = SlugHelper.Normalize(query.TourSlug);


    //    var spec = new TourCatalogForSlugResolutionSpecification(destId.Value);

    //    var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
    //    var tours = (await tourRepo.GetAllAsync(spec)).ToList();

    //    Tour? match = null;
    //    foreach (var t in tours)
    //    {
    //        var candidateTitles = t.Translations
    //            .Where(x => !string.IsNullOrWhiteSpace(x.Title))
    //            .OrderByDescending(x => x.Language == listingLang)
    //            .Select(x => x.Title!.Trim())
    //            .Distinct(StringComparer.OrdinalIgnoreCase)
    //            .ToList();

    //        if (candidateTitles.Count == 0)
    //            continue;

    //        var isMatch = candidateTitles.Any(title =>
    //            SlugHelper.ToUrlSlug(title) == requestSlug
    //            || SlugHelper.Normalize(title) == normalizedRequestSlug);

    //        if (!isMatch)
    //            continue;

    //        match = t;
    //        break;
    //    }

    //    if (match is null)
    //        return Result.Fail(new NotFoundError("Tour not found for this link."));

    //    if ((match.UserType & effectiveUserType) != effectiveUserType)
    //        return Result.Fail(new NotFoundError("Tour not found for this link."));

    //    var todayUtc = DateOnly.FromDateTime(DateTime.UtcNow);

    //    var priceRepo = _unitOfWork.GetRepository<Price, Guid>();

    //    var scheduleRepo = _unitOfWork.GetRepository<TourSchedule, Guid>();
    //    var reviewRepo = _unitOfWork.GetRepository<Review, Guid>();
    //    //var reviewTrRepo = _unitOfWork.GetRepository<ReviewTranslation, Guid>();
    //    //var allowedUserType = NormalizeEffectiveUserType(effectiveUserType);

    //    var prices = (await priceRepo.GetAllAsync(new PricesForTourSpecification(match.Id, effectiveUserType))).ToList();
    //    var schedules = (await scheduleRepo.GetAllAsync(new TourSchedulesForTourSpecification(match.Id))).ToList();
    //    var reviews = (await reviewRepo.GetAllAsync(new ReviewsForTourSpecification(match.Id))).ToList();
    //    //var reviewTranslations = Array.Empty<ReviewTranslation>();
    //    if (reviews.Count > 0)
    //    {
    //        var ids = reviews.Select(r => r.Id).ToList();
    //        //reviewTranslations = (await reviewTrRepo.GetAllAsync(
    //        //    new ReviewTranslationsForReviewsSpecification(ids, listingLang))).ToArray();
    //    }

    //    var ctRepo = _unitOfWork.GetRepository<Cancellation, Guid>();
    //    var ctRow = (await ctRepo.GetByIdAsync(
    //        new CancellationForTourSpecification(match.Id)));

    //    var pick = ctRow is null ? null : ctRow.Translations.FirstOrDefault(x => x.Language == listingLang)
    //                  ?? ctRow.Translations.FirstOrDefault();

    //    string? cancellationPolicyDescription = pick is null ? null : pick.Description;

    //    var countryInfo = await _unitOfWork
    //           .GetRepository<CountryInfo, Guid>()
    //           .GetByIdAsync(new GetCountryInfoByDestinationIdSpecification(destId.Value, listingLang));

    //    var inclusions = await _unitOfWork.GetRepository<TourInclusion, Guid>().GetAllAsync(new GetInclustionWithTourIdSpecification(match.Id));

    //    var destination = await _unitOfWork.GetRepository<Destination, Guid>().GetByIdAsync(new GetDestinationWithTranslationsSpecification(destId.Value));


    //    var detail = UserTourCatalogMapper.ToDetail(
    //        match,
    //        listingLang,
    //        prices,
    //        effectiveUserType,
    //        schedules,
    //        reviews,

    //        todayUtc,
    //        rate.ValueOrDefault,
    //        filteredCurrency,
    //        countryInfo,
    //        inclusions,
    //        ctRow,
    //        destination,
    //        cancellationPolicyDescription,
    //        currentUserId
    //    );

    //    return Result.Ok(detail);
    //}
    private static UserType NormalizeEffectiveUserType(UserType? effectiveUserType)
      => effectiveUserType == UserType.VIP ? UserType.VIP : UserType.Public;
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

    public async Task<Result<IEnumerable<UserTrendingTourItemDto>>> GetTrendingToursAsync(string? language, string? currency, string? userType, int take = 6)
    {
        var listingLang = _currentUserService.Language;
        var filteredCurrency = _currentUserService.Currency;
        var rate = await _currencyRateService.GetRateAsync(
                        Constants.BaseCurrency,
                        filteredCurrency, true);

        if (!rate.IsSuccess)
            return Result.Fail(rate.Errors);

        var effectiveUserType = ParseUserType(userType) ?? UserType.Public;
        var top = take <= 0 ? 6 : Math.Min(take, 24);

        var todayUtc = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var spec = new TrendingToursSpecification(todayUtc);
        var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
        var rows = (await tourRepo.GetAllAsync(spec)).ToList();

        if (rows.Count == 0)
            return Result.Ok<IEnumerable<UserTrendingTourItemDto>>([]);

        var tourIds = rows
              .Select(x => x.Id)
              .ToList();

        var prices =
             await _unitOfWork.PriceRepo
                 .GetTourPriceSummariesAsync(tourIds, effectiveUserType);
        var priceMap = prices
            .GroupBy(p => p.TourId)
            .ToDictionary(
                g => g.Key,
                g => new TourPriceSummaryDto
                {
                    TourId = g.Key,
                    MaxRetailPrice = g.Max(x => x.RetailPrice),
                    MaxCostPrice = g.Max(x => x.CostPrice)
                });

        var reviewRows =
            await _unitOfWork.ReviewRepo
               .GetTourReviewSummariesAsync(tourIds);
        var reviewMap = reviewRows
        .GroupBy(x => x.TourId)
        .ToDictionary(
            g => g.Key,
            g => new TourReviewSummaryDto
            {
                TourId = g.Key,
                ReviewCount = g.Count(),
                AverageRating = g.Average(x => x.Rate)
            });
        var cancellationRows =
            await _unitOfWork.CancellationRepo
                .GetFreeCancellationLookupAsync(tourIds);
        var cancellationMap = cancellationRows
            .GroupBy(x => x.TourId)
            .ToDictionary(
                g => g.Key,
                g => g.Any(x => x.IsFree));

        var mapped = rows
            .Select(t =>
            {
                var tr = t.Translations
                .FirstOrDefault(x => x.Language == listingLang)
                ?? t.Translations.FirstOrDefault();
                var hero = t.Images
                      .Where(i => !i.IsDeleted)
                      .OrderBy(i => i.Id)
                      .FirstOrDefault();

                priceMap.TryGetValue(t.Id, out var price);
                reviewMap.TryGetValue(t.Id, out var review);
                string? originalDisplay = null;
                int? discountPct = null;
                if (price is not null && price.MaxRetailPrice is not null && price.MaxCostPrice is not null && price.MaxCostPrice > price.MaxRetailPrice + 0.0001m)
                {
                    var inferredPct = (int)Math.Round((1 - price.MaxRetailPrice.Value / price.MaxCostPrice.Value) * 100m);
                    if (inferredPct > 0)
                    {
                        discountPct = inferredPct;
                        originalDisplay = $"{filteredCurrency} {Math.Round(price.MaxCostPrice.Value * rate.ValueOrDefault, 2):0.##}";
                    }
                }
                var dur = t.Duration;
                var title = tr?.Title ?? string.Empty;

                var item = new UserTourListItemDto(
                            TourId: t.Id,
                            Title: title,
                            Description: tr?.Description,
                            HeroImageUrl: hero?.ImageUrl,
                            AverageRating: review?.AverageRating,
                            ReviewCount: review?.ReviewCount ?? 0,
                            FreeCancellation: cancellationMap.GetValueOrDefault(t.Id),

                            IsWheelchairAvailable: t.IsWheelchairAvailable,
                            IsPitsAllowed: t.IsPitsAllowed,
                            OriginalPrice: originalDisplay,
                            FromPrice: price is null || price.MaxRetailPrice is null ? null :
                            $"{filteredCurrency} {Math.Round(price.MaxRetailPrice.Value * rate.ValueOrDefault, 2):0.##}",
                            DiscountPercent: discountPct,
                            DurationDisplay: dur.TotalHours >= 24
                                ? $"{(int)dur.TotalDays}d {dur.Hours}h"
                                : dur.TotalHours >= 1
                                    ? $"{(int)dur.TotalHours}h {dur.Minutes}m"
                                    : $"{dur.Minutes}m",
                            GuideLanguage: t.GuideLanguage?.ToString(),
                             TourSlug: SlugHelper.ToUrlSlug(title)
                                        );





                var destinationTranslation = t.Destination.Translations
                    .FirstOrDefault(x => x.Language == listingLang)
                    ?? t.Destination.Translations.FirstOrDefault();
                var destinationName = destinationTranslation?.Name ?? string.Empty;

                return new
                {
                    Item = item,
                    DestinationSlug = SlugHelper.ToUrlSlug(destinationName),
                    Rating = item.AverageRating ?? 0m,
                    BaseCurrency = Constants.BaseCurrency.ToString(),
                    Discount = item.DiscountPercent,
                    BaseAmount = item.OriginalPrice,
                    FromPrice = item.FromPrice
                };
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.DestinationSlug))
            .Where(x => !string.IsNullOrWhiteSpace(x.FromPrice))
            .OrderByDescending(x => x.Item.ReviewCount)
            .ThenByDescending(x => x.Rating)
            .Take(top)
            .Select(x => new UserTrendingTourItemDto(
                TourId: x.Item.TourId,
                Title: x.Item.Title,
                Description: x.Item.Description,
                HeroImageUrl: x.Item.HeroImageUrl,
                AverageRating: x.Item.AverageRating,
                ReviewCount: x.Item.ReviewCount,
                FilteredCurrency: filteredCurrency.ToString(),
                FromPrice: x.FromPrice,
                Discount: x.Discount,
                BaseCurrency: x.BaseCurrency,
                BaseAmount: x.BaseAmount,
                TourSlug: x.Item.TourSlug,
                DestinationSlug: x.DestinationSlug))
            .ToList();

        return Result.Ok<IEnumerable<UserTrendingTourItemDto>>(mapped);
    }

    //public async Task<Result<List<UserTourPriceTierDto>>> GetPriceByActivityTypeAsync( string Id,PiceWithActivityTypeRequestQuery requestDTO, string? userType)
    //{
    //    //var validationResult = await _validationService.ValidateAsync(requestDTO);
    //    //if (!validationResult.IsSuccess)
    //    //    return validationResult;
    //    if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
    //        return Result.Fail("Invalid UUID");

    //    Guid tourId = guid;

    //    var tour = await _unitOfWork.GetRepository<Tour, Guid>().GetByIdAsync(new GetTourByIdWithPriceIncludingOnlySpecification(tourId));

    //    if (tour is null)
    //    {
    //        return Result.Fail(new NotFoundError("This Tour Not Found"));
    //    }
    //    var listingLang = _currentUserService.Language;
    //    var filteredCurrency = _currentUserService.Currency;
    //    var rate = await _currencyRateService.GetRateAsync(
    //                   Constants.BaseCurrency,
    //                   filteredCurrency, true);

    //    if (!rate.IsSuccess)
    //        return Result.Fail(rate.Errors);

    //    var effectiveUserType = ParseUserType(userType) ?? UserType.Public;

    //    var prices = await _unitOfWork.GetRepository<Price, Guid>().GetAllAsync(new PricesForTourSpecification(tourId, effectiveUserType));
    //    var filterdPrices = string.IsNullOrWhiteSpace(requestDTO.ActivityType) ?
    //        tour.Prices
    //            .Where( p => 

    //                p.IsMainActivityType == true

    //                && p.Translations.Any(tr =>
    //                    tr.Language == listingLang))
    //            .OrderBy(x => x.RetailPrice)
    //            .ToList()

    //        :
    //        tour.Prices
    //            .Where(p =>
    //                 p.Translations.Any(tr =>
    //                    tr.Language == listingLang &&
    //                    tr.ActivityType == requestDTO.ActivityType))
    //            .OrderBy(x => x.RetailPrice)
    //            .ToList();
    //    var list = new List<UserTourPriceTierDto>();

    //    foreach (var p in filterdPrices)

    //        {
    //            var tr = p.Translations.FirstOrDefault(x => x.Language == listingLang)
    //                     ?? p.Translations.FirstOrDefault();
    //            var label = tr?.Type ?? "Traveler";
    //            var retail = Math.Round(p.RetailPrice * rate.ValueOrDefault, 2);
    //            var isFree = retail <= 0;
    //            var group = p.UserType.HasFlag(UserType.VIP) ? "VIP" : "Public";
    //            list.Add(new UserTourPriceTierDto(p.Id, label, retail, isFree, group));
    //        }
    //    return list;
    //}




    public Task<Result<List<UserTourPriceTierDto>>> GetPriceByActivityTypeAsync(string id, PiceWithActivityTypeRequestQuery requestDTO, string? userType)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserTourDetailDto>> GetTourByPublicPathAsync(GetTourByPublicPathQuery query, string? userType, string? currentUserId = null)
    {
        throw new NotImplementedException();
    }

    

   

   
}

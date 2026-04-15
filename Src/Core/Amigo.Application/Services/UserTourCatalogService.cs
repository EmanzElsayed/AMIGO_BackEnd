using Amigo.Application.Abstraction.Services;
using Amigo.Application.Helpers;
using Amigo.Application.Mapping;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.Domain.Enum;
using Amigo.Domain.Errors.BusinessErrors;
using Amigo.Application.Specifications.TourSpecification.User;
using Amigo.SharedKernal.DTOs.Results;
using Amigo.SharedKernal.DTOs.Tour;
using Amigo.SharedKernal.QueryParams;

namespace Amigo.Application.Services;

public class UserTourCatalogService(
            IValidationService _validationService,
            IUnitOfWork _unitOfWork,
            IDestinationSlugResolver _slugResolver) 
                : IUserTourCatalogService
{
    public async Task<Result<PaginatedResponse<UserTourListItemDto>>> GetToursAsync(GetUserToursQuery query)
    {
        var validationResult = await _validationService.ValidateAsync(query);
        if (!validationResult.IsSuccess)
            return validationResult;

        var listingLang = string.IsNullOrWhiteSpace(query.Language)
            ? Language.English
            : EnumsMapping.ToLanguageEnum(query.Language!);

        Language? effectiveGuide = query.OnlyInUserLanguage == true
            ? listingLang
            : (string.IsNullOrWhiteSpace(query.GuideLanguage)
                ? null
                : EnumsMapping.ToLanguageEnum(query.GuideLanguage!));

        CountryCode? country = null;
        if (!string.IsNullOrWhiteSpace(query.CountryCode)
            && Enum.TryParse<CountryCode>(query.CountryCode, true, out var cc)
            && cc != CountryCode.None)

            country = cc;
        
        CurrencyCode? currencyFilter = null;

        UserType? userType = ParseUserType(query.UserType); 


        DateOnly? availabilityDate = null;
        if (!string.IsNullOrWhiteSpace(query.AvailabilityDate)
            && DateOnly.TryParse(query.AvailabilityDate, out var ad))
            availabilityDate = ad; // use mapping

        var destId = query.DestinationId;

        var countSpec = new UserTourCatalogFilterSpecification(
            query,
            destId,
            listingLang,
            effectiveGuide,
            currencyFilter,
            country,
            userType,
            availabilityDate);

        var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
        var totalItems = await tourRepo.GetCountSpecificationAsync(countSpec);

        var listSpec = new UserTourCatalogSpecification(
            query,
            destId,
            listingLang,
            effectiveGuide,
            currencyFilter,
            country,
            userType,
            availabilityDate,
            applyPaging: true);

        var tours = (await tourRepo.GetAllAsync(listSpec)).ToList();
        var data = tours.Select(t => UserTourCatalogMapper.ToListItem(t, listingLang, userType)).ToList();

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
        var lang = string.IsNullOrWhiteSpace(language)
            ? Language.English
            : EnumsMapping.ToLanguageEnum(language!);

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

        const double fallbackHours = 24.0;
        var hours = maxHours is > 0 ? maxHours.Value : fallbackHours;

        return Result.Ok(new MaxDurationHoursResponseDto { MaxDurationHours = hours });
    }

    public async Task<Result<UserTourDetailDto>> GetTourByPublicPathAsync(GetTourByPublicPathQuery query, string? userType)
    {
        var validationResult = await _validationService.ValidateAsync(query);
        if (!validationResult.IsSuccess)
            return validationResult;

        var destId = await _slugResolver.ResolveDestinationIdAsync(query.DestinationSlug);
        if (destId is null)
            return Result.Fail(new NotFoundError("Destination not found for this link."));

        var listingLang = string.IsNullOrWhiteSpace(query.Language)
            ? Language.English
            : EnumsMapping.ToLanguageEnum(query.Language!);
        var effectiveUserType = ParseUserType(userType) ?? UserType.Public;

        var requestSlug = SlugHelper.ToUrlSlug(query.TourSlug);
        var normalizedRequestSlug = SlugHelper.Normalize(query.TourSlug);


        var spec = new TourCatalogForSlugResolutionSpecification(destId.Value);

        var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
        var tours = (await tourRepo.GetAllAsync(spec)).ToList();

        Tour? match = null;
        foreach (var t in tours)
        {
            var candidateTitles = t.Translations
                .Where(x => !string.IsNullOrWhiteSpace(x.Title))
                .OrderByDescending(x => x.Language == listingLang)
                .Select(x => x.Title!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (candidateTitles.Count == 0)
                continue;

            var isMatch = candidateTitles.Any(title =>
                SlugHelper.ToUrlSlug(title) == requestSlug
                || SlugHelper.Normalize(title) == normalizedRequestSlug);

            if (!isMatch)
                continue;

            match = t;
            break;
        }

        if (match is null)
            return Result.Fail(new NotFoundError("Tour not found for this link."));

        if ((match.UserType & effectiveUserType) != effectiveUserType)
            return Result.Fail(new NotFoundError("Tour not found for this link."));

        var todayUtc = DateOnly.FromDateTime(DateTime.UtcNow);

        var priceRepo = _unitOfWork.GetRepository<Price, Guid>();

        var scheduleRepo = _unitOfWork.GetRepository<TourSchedule, Guid>();
        var reviewRepo = _unitOfWork.GetRepository<Review, Guid>();
        var reviewTrRepo = _unitOfWork.GetRepository<ReviewTranslation, Guid>();

        var prices = (await priceRepo.GetAllAsync(new PricesForTourSpecification(match.Id))).ToList();
        var schedules = (await scheduleRepo.GetAllAsync(new TourSchedulesForTourSpecification(match.Id))).ToList();
        var reviews = (await reviewRepo.GetAllAsync(new ReviewsForTourSpecification(match.Id))).ToList();
        var reviewTranslations = Array.Empty<ReviewTranslation>();
        if (reviews.Count > 0)
        {
            var ids = reviews.Select(r => r.Id).ToList();
            reviewTranslations = (await reviewTrRepo.GetAllAsync(
                new ReviewTranslationsForReviewsSpecification(ids, listingLang))).ToArray();
        }

        string? cancellationPolicyDescription = null;
        if (match.Cancellation is { IsDeleted: false })
        {
            var ctRepo = _unitOfWork.GetRepository<CancellationTranslation, Guid>();
            var ctRows = (await ctRepo.GetAllAsync(
                new CancellationTranslationsForCancellationSpecification(match.Cancellation.Id))).ToList();
            var pick = ctRows.FirstOrDefault(x => x.Language == listingLang)
                       ?? ctRows.FirstOrDefault();
            cancellationPolicyDescription = pick?.Description;
        }

        var detail = UserTourCatalogMapper.ToDetail(
            match,
            listingLang,
            prices,
            effectiveUserType,
            schedules,
            reviews,
            reviewTranslations,
            todayUtc,
            cancellationPolicyDescription
        );

        return Result.Ok(detail);
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

    public async Task<Result<IEnumerable<UserTrendingTourItemDto>>> GetTrendingToursAsync(string? language, string? currency, string? userType, int take = 6)
    {
        var listingLang = string.IsNullOrWhiteSpace(language)
            ? Language.English
            : EnumsMapping.ToLanguageEnum(language!);
        var effectiveUserType = ParseUserType(userType) ?? UserType.Public;
        var top = take <= 0 ? 6 : Math.Min(take, 24);

        var todayUtc = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var spec = new TrendingToursSpecification(todayUtc);
        var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
        var rows = (await tourRepo.GetAllAsync(spec)).ToList();

        if (rows.Count == 0)
            return Result.Ok<IEnumerable<UserTrendingTourItemDto>>([]);

        var mapped = rows
            .Select(t =>
            {
                var item = UserTourCatalogMapper.ToListItem(t, listingLang, effectiveUserType);
                var allowedUserType = effectiveUserType == UserType.VIP ? UserType.VIP : UserType.Public;
                var baseAmount = t.Prices
                    .Where(p => !p.IsDeleted && (p.UserType & allowedUserType) == allowedUserType)
                    .Select(p => (decimal?)(p.Cost * (1 - p.Discount / 100m)))
                    .Min();
                var tr = t.Destination.Translations
                    .FirstOrDefault(x => x.Language == listingLang)
                    ?? t.Destination.Translations.FirstOrDefault();
                var destinationName = tr?.Name ?? string.Empty;
                return new
                {
                    Item = item,
                    DestinationSlug = SlugHelper.ToUrlSlug(destinationName),
                    Rating = item.AverageRating ?? 0m,
                    BaseCurrency = t.CurrencyCode.ToString(),
                    BaseAmount = baseAmount
                };
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.DestinationSlug))
            .Where(x => x.BaseAmount.HasValue)
            .OrderByDescending(x => x.Item.ReviewCount)
            .ThenByDescending(x => x.Rating)
            .Take(top)
            .Select(x => new UserTrendingTourItemDto(
                TourId: x.Item.TourId,
                Title: x.Item.Title,
                HeroImageUrl: x.Item.HeroImageUrl,
                AverageRating: x.Item.AverageRating,
                ReviewCount: x.Item.ReviewCount,
                FromPrice: x.Item.FromPrice,
                BaseCurrency: x.BaseCurrency,
                BaseAmount: x.BaseAmount,
                TourSlug: x.Item.TourSlug,
                DestinationSlug: x.DestinationSlug))
            .ToList();

        return Result.Ok<IEnumerable<UserTrendingTourItemDto>>(mapped);
    }
}

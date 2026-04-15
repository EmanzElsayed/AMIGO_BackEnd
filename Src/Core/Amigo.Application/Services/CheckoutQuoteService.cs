using Amigo.Application.Abstraction;
using Amigo.Application.Abstraction.Services;
using Amigo.Application.Mapping;
using Amigo.Application.Specifications.TourSpecification.User;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Amigo.Domain.Errors.BusinessErrors;
using Amigo.SharedKernal.DTOs.Results;
using Amigo.SharedKernal.DTOs.Tour;

namespace Amigo.Application.Services;

public class CheckoutQuoteService(
    IValidationService _validationService,
    IUnitOfWork _unitOfWork) : ICheckoutQuoteService
{
    public async Task<Result<CheckoutQuoteResponseDto>> QuoteAsync(CheckoutQuoteRequestDto request)
    {
        var validationResult = await _validationService.ValidateAsync(request);
        if (!validationResult.IsSuccess)
            return validationResult;

        if (!DateOnly.TryParse(request.DateIso, out var targetDay))
            return Result.Fail<CheckoutQuoteResponseDto>("Invalid date.");

        var todayUtc = DateOnly.FromDateTime(DateTime.UtcNow);
        if (targetDay < todayUtc)
            return Result.Fail<CheckoutQuoteResponseDto>("Date is in the past.");

        var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
        var tour = await tourRepo.GetByIdAsync(request.TourId);
        if (tour is null || tour.IsDeleted)
            return Result.Fail(new NotFoundError("Tour not found."));

        var listingLang = string.IsNullOrWhiteSpace(request.Language)
            ? Language.English
            : EnumsMapping.ToLanguageEnum(request.Language!);
        CurrencyCode? requestedCurrency = null;
        if (!string.IsNullOrWhiteSpace(request.Currency)
            && Enum.TryParse<CurrencyCode>(request.Currency, true, out var ccy)
            && ccy != CurrencyCode.None)
            requestedCurrency = ccy;
        if (requestedCurrency.HasValue && tour.CurrencyCode != requestedCurrency.Value)
            return Result.Fail<CheckoutQuoteResponseDto>("Selected currency does not match this tour currency.");
        var effectiveUserType = ParseUserType(request.EffectiveUserType) ?? UserType.Public;

        var scheduleRepo = _unitOfWork.GetRepository<TourSchedule, Guid>();
        var schedules = (await scheduleRepo.GetAllAsync(new TourSchedulesForTourSpecification(request.TourId))).ToList();

        if (!IsSlotAvailableOnDate(schedules, targetDay, request.SlotId, todayUtc))
            return Result.Fail<CheckoutQuoteResponseDto>("This time is not available for the selected date.");

        var priceRepo = _unitOfWork.GetRepository<Price, Guid>();
        var prices = (await priceRepo.GetAllAsync(new PricesForTourSpecification(request.TourId))).ToList();

        var lineDtos = new List<CheckoutQuoteTierLineDto>();
        decimal total = 0;

        foreach (var row in request.Tiers)
        {
            if (row.Quantity < 0)
                return Result.Fail<CheckoutQuoteResponseDto>("Invalid passenger count.");

            var price = prices.FirstOrDefault(p => p.Id == row.PriceId);
            if (price is null)
                return Result.Fail<CheckoutQuoteResponseDto>("Unknown price tier for this tour.");

            if ((price.UserType & effectiveUserType) != effectiveUserType)
                return Result.Fail<CheckoutQuoteResponseDto>("Selected price tier is not available for your account type.");

            var tr = price.Translations.FirstOrDefault(x => x.Language == listingLang)
                     ?? price.Translations.FirstOrDefault();
            var label = tr?.Type ?? "Traveler";
            var retail = price.RetailPrice;
            var isFree = retail <= 0;
            var lineTotal = isFree ? 0 : retail * row.Quantity;
            total += lineTotal;

            lineDtos.Add(new CheckoutQuoteTierLineDto(
                price.Id,
                label,
                retail,
                row.Quantity,
                lineTotal,
                isFree));
        }

        return Result.Ok(new CheckoutQuoteResponseDto(
            total,
            tour.CurrencyCode.ToString(),
            lineDtos));
    }

    private static bool IsSlotAvailableOnDate(
        IReadOnlyList<TourSchedule> schedules,
        DateOnly target,
        Guid slotId,
        DateOnly todayUtc)
    {
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
                if (day < todayUtc || day != target)
                    continue;

                foreach (var slot in schedule.AvailableSlots.Where(s => !s.IsDeleted))
                {
                    if (slot.AvailableTimeStatus != AvailableDateTimeStatus.Available)
                        continue;
                    if (slot.Id == slotId)
                        return true;
                }
            }
        }

        return false;
    }

    private static UserType? ParseUserType(string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return null;
        if (s.Equals("VIP", StringComparison.OrdinalIgnoreCase))
            return UserType.VIP;
        if ( s.Equals("Public", StringComparison.OrdinalIgnoreCase))
            return UserType.Public;
        return null;
    }
}

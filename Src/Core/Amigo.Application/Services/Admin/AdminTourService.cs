using Amigo.Application.Helpers;
using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Application.Specifications.TourSpecification;
using Amigo.Application.Specifications.TourSpecification.Admin;
using Amigo.Application.Specifications.TourSpecification.User;
using Amigo.Application.Specifications.OrderSpecification;
using Amigo.Application.Specifications.AvailableSlotsSpecification;
using Amigo.Domain.DTO.AvailableSlots;
using Amigo.Domain.DTO.Cancellation;
using Amigo.Domain.DTO.Images;
using Amigo.Domain.DTO.Price;
using Amigo.Domain.DTO.Tour;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.DTOs.Tour;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Amigo.Domain.DTO.Translation;
using Amigo.Application.BackgroundTasks;

namespace Amigo.Application.Services.Admin
{
    public class AdminTourService : IAdminTourService
    {
        private static readonly IReadOnlyDictionary<SupportedLanguage, string> LanguageCodeMap =
            new Dictionary<SupportedLanguage, string>
            {
                { SupportedLanguage.en, "en"    },
                { SupportedLanguage.es, "es"    },
                { SupportedLanguage.fr, "fr"    },
                { SupportedLanguage.it, "it"    },
                { SupportedLanguage.pt, "pt"    },
                { SupportedLanguage.br, "pt-br" }, // was "pt-BR" – must be lowercase
            };

        private readonly IValidationService _validationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAdminPriceService _adminPriceService;
        private readonly IAdminTourScheduleService _adminTourScheduleService;
        private readonly IAdminTourInclusionService _adminTourInclusionService;
        private readonly IAdminTourCancellationService _adminTourCancellationService;
        private readonly IImageService _imageService;
        private readonly ImageCloudService _imageCloud;
        private readonly ITranslationService _translationService;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly ILogger<AdminTourService> _logger;

        public AdminTourService(
            IValidationService validationService,
            IUnitOfWork unitOfWork,
            IAdminPriceService adminPriceService,
            IAdminTourScheduleService adminTourScheduleService,
            IAdminTourInclusionService adminTourInclusionService,
            IAdminTourCancellationService adminTourCancellationService,
            IImageService imageService,
            ImageCloudService imageCloud,
            ITranslationService translationService,
            IBackgroundTaskQueue backgroundTaskQueue,
            ILogger<AdminTourService> logger)
        {
            _validationService = validationService;
            _unitOfWork = unitOfWork;
            _adminPriceService = adminPriceService;
            _adminTourScheduleService = adminTourScheduleService;
            _adminTourInclusionService = adminTourInclusionService;
            _adminTourCancellationService = adminTourCancellationService;
            _imageService = imageService;
            _imageCloud = imageCloud;
            _translationService = translationService;
            _backgroundTaskQueue = backgroundTaskQueue;
            _logger = logger;
        }

        
        public async Task<Result<CreateTourResponseDTO>> CreateTourAsync(CreateTourRequestDTO requestDTO)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
                return validationResult;

            var destination = await _unitOfWork
                .GetRepository<Destination, Guid>()
                .GetByIdAsync(new GetNotDeletedDestinationByIdSpecification(requestDTO.DestinationId));

            if (destination is null)
                return Result.Fail(new NotFoundError("This Destination Not Found"));

            UserType userType = requestDTO.UserType is null
                ? UserType.Public
                : requestDTO.UserType.Aggregate(UserType.None, (acc, type) => acc | type);

            if (requestDTO.Prices is not null && requestDTO.Prices.Any(p =>
                    p.UserType == UserType.None || (p.UserType & userType) != p.UserType))
                return Result.Fail("Invalid pricing tiers: tier user type must match selected tour audience.");

            var tour = TourMapping.TourToEntity(requestDTO, destination, userType);
            var tourTranslations = TourMapping.TourTranslationsToEntity(requestDTO, tour);
            var tourImages = new List<TourImage>();

            if (requestDTO.Images is not null && requestDTO.Images.Any())
            {
                tourImages = ImageMapping.ImagesToEntity(requestDTO.Images, tour).ToList();
                tour.Images.Select(i => _imageCloud.DeleteImage(i.ImagePublicId));
            }

            var tourPrices = requestDTO.Prices is not null && requestDTO.Prices.Any()
                ? PriceMapping.PricesDTOToEntity(requestDTO.Prices, tour, requestDTO.Language)
                : new List<Price>();

            var tourSchedule = requestDTO.Schedule is not null && requestDTO.Schedule.Any()
                ? TourScheduleMapping.TourSchedulesDTOToEntity(requestDTO.Schedule, tour)
                : new List<TourSchedule>();

            var tourInclusion =
                (requestDTO.Includes is not null && requestDTO.Includes.Any()) ||
                (requestDTO.NotIncludes is not null && requestDTO.NotIncludes.Any())
                    ? InclusionMapping.TourInclusionToEntity(requestDTO.Includes, requestDTO.NotIncludes, tour, requestDTO.Language)
                    : new List<TourInclusion>();

            var cancellation = requestDTO.Cancellation is not null
                ? CancellationMapping.CancellationToEntity(requestDTO.Cancellation, tour, requestDTO.Language)
                : new Cancellation();

            var strategy = _unitOfWork.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    await _unitOfWork.GetRepository<Tour, Guid>().AddAsync(tour);

                    if (tourTranslations.Any())
                        await _unitOfWork.GetRepository<TourTranslation, Guid>().AddRangeAsync(tourTranslations);

                    if (tourImages.Any())
                        await _unitOfWork.GetRepository<TourImage, Guid>().AddRangeAsync(tourImages);

                    if (tourPrices.Any())
                        await _unitOfWork.GetRepository<Price, Guid>().AddRangeAsync(tourPrices);

                    if (tourSchedule.Any())
                        await _unitOfWork.GetRepository<TourSchedule, Guid>().AddRangeAsync(tourSchedule);

                    if (tourInclusion is not null && tourInclusion.Any())
                        await _unitOfWork.GetRepository<TourInclusion, Guid>().AddRangeAsync(tourInclusion);

                    if (cancellation is not null)
                        await _unitOfWork.GetRepository<Cancellation, Guid>().AddAsync(cancellation);

                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var sourceLanguage = EnumsMapping.ToLanguageEnum(requestDTO.Language);
                    var sourceTranslation = tourTranslations.FirstOrDefault(t => t.Language == sourceLanguage);

                    var capturedTourId = tour.Id;
                    var capturedTitle = sourceTranslation?.Title ?? "";
                    var capturedDescription = sourceTranslation?.Description ?? "";
                    var capturedLanguage = sourceLanguage;

                    _ = _backgroundTaskQueue.EnqueueAsync(async (serviceProvider, cancellationToken) =>
                    {
                        await TranslateTourAsync(
                            capturedTourId,
                            capturedTitle,
                            capturedDescription,
                            capturedLanguage,
                            serviceProvider,
                            cancellationToken);
                    });

                    return Result.Ok(new CreateTourResponseDTO(tour.Id))
                                 .WithSuccess(new Success("Tour Created Successfully")
                                 .WithMetadata("StatusCode", (int)HttpStatusCode.Created));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return FluentValidationExtension.FromException(details: ex.Message);
                }
            });
        }
        private static async Task TranslateTourAsync(
            Guid tourId,
            string tourTitle,
            string tourDescription,
            SupportedLanguage sourceLanguage,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            var translationService = serviceProvider.GetRequiredService<ITranslationService>();
            var logger = serviceProvider.GetRequiredService<ILogger<AdminTourService>>();

            var targetLanguages = LanguageCodeMap
                .Where(kv => kv.Key != sourceLanguage)
                .ToList();

            var targetCodes = targetLanguages.Select(kv => kv.Value).ToList();

            logger.LogInformation(
                "[Translation] Tour {TourId}: starting → [{Codes}]",
                tourId, string.Join(", ", targetCodes));

            var tourTranslations = (await unitOfWork
                .GetRepository<TourTranslation, Guid>()
                .GetAllAsync(new TourTranslationsByTourIdSpec(tourId)))
                .ToList();

            var inclusions = (await unitOfWork
                .GetRepository<TourInclusion, Guid>()
                .GetAllAsync(new TourInclusionsByTourIdSpec(tourId)))
                .ToList();

            var cancellations = (await unitOfWork
                .GetRepository<Cancellation, Guid>()
                .GetAllAsync(new CancellationsByTourIdSpec(tourId)))
                .ToList();

            var prices = (await unitOfWork
                .GetRepository<Price, Guid>()
                .GetAllAsync(new PricesByTourIdSpec(tourId)))
                .ToList();

            var allTexts = new HashSet<string>(StringComparer.Ordinal);

            void Add(string? s) { if (!string.IsNullOrWhiteSpace(s)) allTexts.Add(s); }

            Add(tourTitle);
            Add(tourDescription);

            foreach (var inc in inclusions)
                Add(inc.Translations.FirstOrDefault(t => t.Language == sourceLanguage)?.Text);

            foreach (var can in cancellations)
                Add(can.Translations.FirstOrDefault(t => t.Language == sourceLanguage)?.Description);

            foreach (var price in prices)
            {
                var src = price.Translations.FirstOrDefault(t => t.Language == sourceLanguage);
                Add(src?.Type);
                Add(src?.ActivityType);
            }

            logger.LogInformation(
                "[Translation] Tour {TourId}: {Count} unique texts → ONE batch API call.",
                tourId, allTexts.Count);

            var batchResult = await translationService.TranslateBatchAsync(
                allTexts.ToList(),
                targetCodes,
                cancellationToken);

            string Get(string? source, string langCode)
            {
                if (string.IsNullOrWhiteSpace(source)) return string.Empty;

                if (batchResult.TryGetValue(source, out var byLang) &&
                    byLang.TryGetValue(langCode, out var translated) &&
                    !string.IsNullOrWhiteSpace(translated))
                    return translated;

                logger.LogWarning(
                    "[Translation] Missing: lang='{Lang}', text='{Snip}'",
                    langCode, source.Length > 60 ? source[..60] + "…" : source);

                return source; // fallback: keep source text
            }

            foreach (var (langEnum, langCode) in targetLanguages)
            {
                var tourTrans = tourTranslations.FirstOrDefault(tt => tt.Language == langEnum);
                if (tourTrans is not null)
                {
                    tourTrans.Title = Get(tourTitle, langCode);
                    tourTrans.Description = Get(tourDescription, langCode);
                    unitOfWork.GetRepository<TourTranslation, Guid>().Update(tourTrans);
                }
                else
                {
                    logger.LogWarning(
                        "[Translation] No TourTranslation row for lang {Lang} – skipping.", langEnum);
                }

                foreach (var inc in inclusions)
                {
                    var src = inc.Translations.FirstOrDefault(t => t.Language == sourceLanguage)?.Text;
                    var target = inc.Translations.FirstOrDefault(t => t.Language == langEnum);
                    if (target is not null) 
                    {
                        target.Text = Get(src, langCode);
                        unitOfWork.GetRepository<InclusionTranslation, Guid>().Update(target);
                    }
                }

                foreach (var can in cancellations)
                {
                    var src = can.Translations.FirstOrDefault(t => t.Language == sourceLanguage)?.Description;
                    var target = can.Translations.FirstOrDefault(t => t.Language == langEnum);
                    if (target is not null) 
                    {
                        target.Description = Get(src, langCode);
                        unitOfWork.GetRepository<CancellationTranslation, Guid>().Update(target);
                    }
                }

                foreach (var price in prices)
                {
                    var srcTrans = price.Translations.FirstOrDefault(t => t.Language == sourceLanguage);
                    var targetTrans = price.Translations.FirstOrDefault(t => t.Language == langEnum);
                    if (targetTrans is null || srcTrans is null) continue;

                    targetTrans.Type = Get(srcTrans.Type, langCode);
                    targetTrans.ActivityType = Get(srcTrans.ActivityType, langCode);
                    unitOfWork.GetRepository<PriceTranslation, Guid>().Update(targetTrans);
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "[Translation] Tour {TourId}: all translations saved.", tourId);
        }

        
        public TourTranslationItem CreateTourTranslationItem(
            SupportedLanguage sourceLanguage,
            Tour tour,
            List<TourInclusion> tourInclusions,
            Cancellation cancellation,
            List<Price> tourPrice,
            Destination destination)
        {
            var tourTranslation = tour.Translations.Where(t => t.Language == sourceLanguage);
            var destinationTranslation = destination.Translations.Where(t => t.Language == sourceLanguage);
            var cancellationTranslation = cancellation.Translations.Where(t => t.Language == sourceLanguage);

            return new TourTranslationItem
            {
                TourId = tour.Id,
                SourceLanguage = sourceLanguage,
                Title = tourTranslation.Select(t => t.Title).FirstOrDefault(),
                Description = tourTranslation.Select(t => t.Description).FirstOrDefault(),

                Destination = new DestinationTranslationItem
                {
                    DestinationId = destination.Id,
                    Name = destinationTranslation.Select(d => d.Name).FirstOrDefault()
                },

                Cancellation = new CancellationTranslationItem
                {
                    CancellationId = cancellation.Id,
                    Description = cancellationTranslation.Select(c => c.Description).FirstOrDefault()
                },

                Inclusions = tourInclusions
                    .Select(i => new InclusionTranslationItem
                    {
                        InclusionId = i.Id,
                        Text = i.Translations
                            .Where(t => t.Language == sourceLanguage)
                            .Select(t => t.Text)
                            .FirstOrDefault() ?? ""
                    })
                    .ToList()
            };
        }

     
        public async Task<Result<PaginatedResponse<AdminTourListItemResponseDTO>>> GetAllToursAsync(
            GetAllAdminTourQuery requestQuery)
        {
            var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
            var tours = await tourRepo.GetAllAsync(new GetAllToursForAdminSpecification(requestQuery));

            SupportedLanguage language = !string.IsNullOrWhiteSpace(requestQuery.Language)
                ? EnumsMapping.ToLanguageEnum(requestQuery.Language)
                : Constants.BaseLanguage;

            var bookedRepo = _unitOfWork.GetRepository<Booking, Guid>();
            var tourIds = tours.Select(t => t.Id).ToList();
            var bookedData = await bookedRepo.GetAllAsync(new GetBookingsByTourIdsSpecification(tourIds));

            var travelersCountByTourId = bookedData
                .Where(b => b.OrderItem?.TourId != null)
                .GroupBy(b => b.OrderItem!.TourId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(b => b.Travelers?.Count ?? 0));

            var tourCapacity = tours.ToDictionary(
                t => t.Id,
                t => t.AvailableTimes.SelectMany(at => at.AvailableSlots).Sum(s => s.MaxCapacity));

            var mappingTours = tours.Select(tour =>
            {
                var vipPrice = tour.Prices
                    .Where(p => !p.IsDeleted && p.UserType == UserType.VIP &&
                                (p.IsMainActivityType == null || p.IsMainActivityType == true))
                    .OrderByDescending(p => p.RetailPrice)
                    .FirstOrDefault();

                var publicPrice = tour.Prices
                    .Where(p => !p.IsDeleted && p.UserType == UserType.Public &&
                                (p.IsMainActivityType == null || p.IsMainActivityType == true))
                    .OrderByDescending(p => p.RetailPrice)
                    .FirstOrDefault();

                var translation = tour.Translations.FirstOrDefault(t => t.Language == language);
                var destinationTranslation = tour.Destination.Translations.FirstOrDefault(t => t.Language == language);

                return new AdminTourListItemResponseDTO
                {
                    TourId = tour.Id,
                    Title = translation?.Title ?? "",
                    DestinationName = destinationTranslation?.Name ?? "",
                    ImageUrl = tour.Images.FirstOrDefault()?.ImageUrl,

                    EntryAmountVIP = vipPrice?.RetailPrice ?? 0,
                    EntryAmountVIPLabel = vipPrice?.Translations
                        .FirstOrDefault(t => t.Language == language)?.Type ?? "",

                    EntryAmountPublic = publicPrice?.RetailPrice ?? 0,
                    EntryAmountPublicLabel = publicPrice?.Translations
                        .FirstOrDefault(t => t.Language == language)?.Type ?? "",

                    TotalCapacity = tour.AvailableTimes
                        .SelectMany(t => t.AvailableSlots)
                        .Where(s => s.AvailableTimeStatus == AvailableDateTimeStatus.Available)
                        .Sum(s => s.MaxCapacity),

                    BookedSeats = travelersCountByTourId.GetValueOrDefault(tour.Id, 0),

                    BookedPercentage = tourCapacity[tour.Id] > 0
                        ? (double)travelersCountByTourId.GetValueOrDefault(tour.Id, 0) / tourCapacity[tour.Id] * 100
                        : 0
                };
            });

            var countTourData = await tourRepo.GetCountSpecificationAsync(
                new CountGetAllTourForAdminSpecification(requestQuery));

            return Result.Ok(new PaginatedResponse<AdminTourListItemResponseDTO>
            {
                Data = mappingTours,
                PageNumber = requestQuery.PageNumber,
                PageSize = requestQuery.PageSize,
                TotalItems = countTourData
            });
        }

      
        public async Task<Result<GetTourResponseDTO>> GetTourById(string Id, GetTourByIdRequestDTO requestDTO)
        {
            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            SupportedLanguage language = Constants.BaseLanguage;
            if (!string.IsNullOrWhiteSpace(requestDTO.Language))
                language = EnumsMapping.ToLanguageEnum(requestDTO.Language);

            var tour = await _unitOfWork
                .GetRepository<Tour, Guid>()
                .GetByIdAsync(new GetTourByIdSpecification(guid));

            if (tour is null)
                return Result.Fail(new NotFoundError("This Tour Not Found"));

            return Result.Ok(MapTourToResponseDTO(tour, language));
        }

       
        public async Task<Result> UpdateTourAsync(UpdateTourRequestDTO requestDTO, string Id)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
                return validationResult;

            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
            var strategy = _unitOfWork.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var tour = await tourRepo.GetByIdAsync(new GetTourByIdSpecification(guid));
                    if (tour is null)
                        return Result.Fail(new NotFoundError("This Tour Not Found"));

                    SupportedLanguage? languageEnum = null;
                    TourTranslation? translation = null;

                    if (!string.IsNullOrWhiteSpace(requestDTO.Language))
                    {
                        languageEnum = EnumsMapping.ToLanguageEnum(requestDTO.Language);
                        translation = tour.Translations.FirstOrDefault(t => t.Language == languageEnum);
                    }

                    if (requestDTO.DestinationId is not null)
                    {
                        var destination = await _unitOfWork
                            .GetRepository<Destination, Guid>()
                            .GetByIdAsync(new GetNotDeletedDestinationByIdSpecification(requestDTO.DestinationId.Value));

                        if (destination is null)
                            return Result.Fail(new NotFoundError("This Destination Not Found"));

                        tour.Destination = destination;
                        tour.DestinationId = destination.Id;
                    }

                    TourMapping.UpdateTour(requestDTO, tour, translation, languageEnum);

                    if (requestDTO.Prices is not null && requestDTO.Prices.Any())
                        await _adminPriceService.UpdatePricesAsync(tour, requestDTO.Prices, languageEnum);

                    if (requestDTO.Schedule is not null && requestDTO.Schedule.Any())
                        await _adminTourScheduleService.UpdateScheduleAsync(tour, requestDTO.Schedule);

                    if ((requestDTO.Includes is not null && requestDTO.Includes.Any()) ||
                        (requestDTO.NotIncludes is not null && requestDTO.NotIncludes.Any()))
                        await _adminTourInclusionService.UpdateInclusionAsync(tour, requestDTO.Includes, requestDTO.NotIncludes, languageEnum);

                    if (requestDTO.Cancellation is not null)
                        await _adminTourCancellationService.UpdateCancellationAsync(tour, requestDTO.Cancellation, languageEnum);

                    if (requestDTO.Images is not null && requestDTO.Images.Any())
                        await _imageService.UpdateImagesAsync(tour, requestDTO.Images);

                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Result.Ok().WithSuccess(new Success("Tour Updated Successfully"));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return FluentValidationExtension.FromException(details: ex.Message);
                }
            });
        }

        public GetTourResponseDTO MapTourToResponseDTO(Tour tour, SupportedLanguage language)
        {
            var translation = tour.Translations.FirstOrDefault(t => t.Language == language);
            var destinationTranslation = tour.Destination?.Translations.FirstOrDefault(t => t.Language == language);
            var cancellation = tour.Cancellation;

            return new GetTourResponseDTO(
                Id: tour.Id,
                GuideLanguage: tour.GuideLanguage.ToString() ?? "",
                MeetingPoint: tour.MeetingPoint,

                Images: tour.Images?
                    .Select(i => new GetImageUrlResponseDTO(i.Id, i.ImageUrl))
                    .ToList(),

                Duration: tour.Duration,
                DestinationId: tour.DestinationId,
                Title: translation?.Title ?? "",
                Description: translation?.Description ?? "",
                Language: language.ToString(),
                Currency: tour.CurrencyCode.ToString(),
                UserType: tour.UserType.ToString(),

                Cancellation: cancellation is null ? null : new GetCancellationResponseDTO(
                    Id: cancellation.Id,
                    CancelationPolicyType: cancellation.CancelationPolicyType.ToString(),
                    CancellationBefore: cancellation.CancellationBefore,
                    RefundPercentage: cancellation.RefundPercentage,
                    Description: cancellation.Translations
                        .FirstOrDefault(t => t.Language == language)?.Description ?? ""),

                Includes: tour.TourInclusions
                    .Where(x => x.IsIncluded)
                    .Select(x => x.Translations.FirstOrDefault(t => t.Language == language)?.Text ?? "")
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList(),

                NotIncludes: tour.TourInclusions
                    .Where(x => !x.IsIncluded)
                    .Select(x => x.Translations.FirstOrDefault(t => t.Language == language)?.Text ?? "")
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList(),

                Prices: tour.Prices?
                    .Select(p => new GetPriceResponseDTO(
                        Id: p.Id,
                        Discount: p.Discount,
                        Type: p.Translations.FirstOrDefault(t => t.Language == language)?.Type ?? "",
                        Cost: p.Cost,
                        UserType: p.UserType,
                        IsMainActivityType: p.IsMainActivityType,
                        ActivityType: p.Translations.FirstOrDefault(t => t.Language == language)?.ActivityType ?? ""))
                    .ToList(),

                Schedule: tour.AvailableTimes?
                    .Select(s => new GetTourScheduleResponseDTO(
                        Id: s.Id,
                        AvailableDateStatus: s.AvailableDateStatus.ToString(),
                        StartDate: s.StartDate,
                        availableSlots: s.AvailableSlots?
                            .Select(slot => new GetAvaialbleSlotResponseDTO(
                                slot.Id,
                                slot.StartTime,
                                slot.AvailableTimeStatus.ToString(),
                                slot.MaxCapacity,
                                slot.SlotReservations
                                    .Where(r => !r.IsDeleted && r.Status == ReservationStatus.Confirmed)
                                    .Sum(r => r.Quantity)))
                            .ToList()))
                    .ToList(),

                IsPitsAllowed: tour.IsPitsAllowed,
                IsWheelchairAvailable: tour.IsWheelchairAvailable
            );
        }

      
        public async Task<Result<object>> GetActivityStatsAsync()
        {
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var nextMonthStart = monthStart.AddMonths(1);

            var bookingRepo = _unitOfWork.GetRepository<Booking, Guid>();
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var slotRepo = _unitOfWork.GetRepository<AvailableSlots, Guid>();
            var reservationRepo = _unitOfWork.GetRepository<SlotReservation, Guid>();

            var bookingsThisMonth = await bookingRepo.GetAllAsync(new GetBookingsByDateSpecification(monthStart, nextMonthStart));
            var bookingsCount = bookingsThisMonth.Count();

            var orders = await orderRepo.GetAllAsync(new GetOrdersByDateAndStatusSpecification(monthStart, nextMonthStart, OrderStatus.Paid));
            var grossRevenue = orders.Sum(o => o.TotalAmount);

            var slotsThisMonth = await slotRepo.GetAllAsync(new GetSlotsByDateSpecification(monthStart, nextMonthStart));
            var totalCapacity = slotsThisMonth.Sum(s => s.MaxCapacity);

            var confirmedReservationsThisMonth = await reservationRepo.GetAllAsync(new GetConfirmedReservationsByDateSpecification(monthStart, nextMonthStart));
            var totalBookedSeats = confirmedReservationsThisMonth.Sum(r => r.Quantity);

            var avgCapacity = totalCapacity <= 0
                ? 0
                : Math.Clamp((int)Math.Round((decimal)totalBookedSeats * 100m / totalCapacity, MidpointRounding.AwayFromZero), 0, 100);

            var status = avgCapacity >= 90 ? "Low Stock" : "Active";

            var dailyRevenue = new decimal[DateTime.DaysInMonth(now.Year, now.Month)];
            foreach (var order in orders)
                if (order.OrderDate.HasValue)
                    dailyRevenue[order.OrderDate.Value.Day - 1] += order.TotalAmount;

            var bookingsPerDestination = bookingsThisMonth
                .Where(b => b.OrderItem != null && b.OrderItem.DestinationName != null)
                .GroupBy(b => b.OrderItem.DestinationName)
                .Select(g => new { Label = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(4)
                .ToList();

            int totalBookings = bookingsCount > 0 ? bookingsCount : 1;

            var regionalMix = bookingsPerDestination
                .Select(x => new
                {
                    x.Label,
                    Pct = (int)Math.Round((decimal)x.Count * 100m / totalBookings),
                    x.Count
                })
                .ToList<object>();

            if (!regionalMix.Any())
                regionalMix.Add(new { Label = "Global", Pct = 100, Count = 0 });

            return Result.Ok<object>(new
            {
                bookingsThisMonth = bookingsCount,
                avgCapacity,
                grossRevenue,
                status,
                dailyRevenue,
                regionalMix
            });
        }
    }
}
using Amigo.Application.Abstraction.Services;
using Amigo.Application.BackgroundTasks;
using Amigo.Application.Helpers;
using Amigo.Application.Specifications.AvailableSlotsSpecification;
using Amigo.Application.Specifications.BlackoutDateSpecification;
using Amigo.Application.Specifications.BlackoutWeekDaysSpecification;
using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Application.Specifications.CountriesInfo;
using Amigo.Application.Specifications.OrderSpecification;
using Amigo.Application.Specifications.TourSpecification;
using Amigo.Application.Specifications.TourSpecification.Admin;
using Amigo.Application.Specifications.TourSpecification.User;
using Amigo.Domain.DTO.AvailableSlots;
using Amigo.Domain.DTO.BlackoutDate;
using Amigo.Domain.DTO.BlackoutWeekDays;
using Amigo.Domain.DTO.Cancellation;
using Amigo.Domain.DTO.Images;
using Amigo.Domain.DTO.Price;
using Amigo.Domain.DTO.Tour;
using Amigo.Domain.DTO.Translation;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.DTOs.Tour;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

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
            ILogger<AdminTourService> logger
           
            )
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

            if (requestDTO.IsFullTime == true && requestDTO.Schedule is not null && requestDTO.Schedule.Any())
            {
                return Result.Fail("You Should Select IsFullTime Or Enter Available Time Not Both");

            }

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
                : new List<AvailableSlots>();

            var tourInclusion =
                (requestDTO.Includes is not null && requestDTO.Includes.Any()) ||
                (requestDTO.NotIncludes is not null && requestDTO.NotIncludes.Any())
                    ? InclusionMapping.TourInclusionToEntity(requestDTO.Includes, requestDTO.NotIncludes, tour, requestDTO.Language)
                    : new List<TourInclusion>();

            var cancellations = (requestDTO.Cancellation is not null && requestDTO.Cancellation.Any())
                ? CancellationMapping.CancellationToEntity(requestDTO.Cancellation, tour, requestDTO.Language)
                : new List<Cancellation>();


            var blackoutDates = (requestDTO.BlackoutDates is not null && requestDTO.BlackoutDates.Any())
                 ? TourMapping.BlackoutDatesToEntity(requestDTO.BlackoutDates, tour) : new List<BlackoutDate>();

            var blackoutWeekDayes = (requestDTO.BlackoutWeekDays is not null && requestDTO.BlackoutWeekDays.Any())
                ? TourMapping.BlackoutWeekDaysToEntity(requestDTO.BlackoutWeekDays, tour) : new List<BlackoutWeekDay>();



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
                        await _unitOfWork.GetRepository<AvailableSlots, Guid>().AddRangeAsync(tourSchedule);

                    if (tourInclusion is not null && tourInclusion.Any())
                        await _unitOfWork.GetRepository<TourInclusion, Guid>().AddRangeAsync(tourInclusion);

                    if (cancellations.Any())
                        await _unitOfWork.GetRepository<Cancellation, Guid>().AddRangeAsync(cancellations);

                    if (blackoutDates.Any())
                        await _unitOfWork.GetRepository<BlackoutDate, Guid>().AddRangeAsync(blackoutDates);

                    if (blackoutWeekDayes.Any())
                        await _unitOfWork.GetRepository<BlackoutWeekDay, Guid>().AddRangeAsync(blackoutWeekDayes);


                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var sourceLanguage = EnumsMapping.ToLanguageEnum(requestDTO.Language);

                    var inputTranslate = new TourTranslationItem()
                    {
                        TourId = tour.Id,
                        SourceLanguage = sourceLanguage,
                        Title = requestDTO.Title,
                        Description = requestDTO.Description
                       
                        ,
                        

                        Inclusions = tourInclusion.Select(i => new InclusionTranslationItem()
                        {
                            InclusionId = i.Id,
                            Text = i.Translations.Where(t => t.Language == sourceLanguage)
                                        .Select(t => t.Text)
                                        .FirstOrDefault() ?? ""
                        }).ToList(),
                        Prices = tourPrices.Select(p => new PriceTranslationItem()
                        {
                            PriceId = p.Id,
                            Type = p.Translations.Where(t => t.Language == sourceLanguage)
                                        .Select(t => t.Type)
                                        .FirstOrDefault() ?? "",
                            ActivityType = p.Translations.Where(t => t.Language == sourceLanguage)
                                        .Select(t => t.ActivityType)
                                        .FirstOrDefault() ?? ""

                        }).ToList()
                    };
                    //var sourceTranslation = tourTranslations.FirstOrDefault(t => t.Language == sourceLanguage);

                    //var capturedTourId = tour.Id;
                    //var capturedTitle = sourceTranslation?.Title ?? "";
                    //var capturedDescription = sourceTranslation?.Description ?? "";
                    //var capturedLanguage = sourceLanguage;

                    _ = _backgroundTaskQueue.EnqueueAsync(async (serviceProvider, cancellationToken) =>
                    {


                        var autoTranslationService =
                            serviceProvider.GetRequiredService<IAutoTranslationService>();

                        await autoTranslationService.TranslateTour(
                            sourceLanguage,
                            inputTranslate);
                        //await TranslateTourAsync(
                        //    capturedTourId,
                        //    capturedTitle,
                        //    capturedDescription,
                        //    capturedLanguage,
                        //    serviceProvider,
                        //    cancellationToken);
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

            return new TourTranslationItem
            {
                TourId = tour.Id,
                SourceLanguage = sourceLanguage,
                Title = tourTranslation.Select(t => t.Title).FirstOrDefault(),
                Description = tourTranslation.Select(t => t.Description).FirstOrDefault(),

              

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

        public Task<Result<object>> GetActivityStatsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Result<PaginatedResponse<AdminTourListItemResponseDTO>>> GetAllToursAsync(
            GetAllAdminTourQuery requestQuery)
        {
            var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
            var tours = await tourRepo.GetAllAsync(new GetAllToursForAdminSpecification(requestQuery));

            SupportedLanguage language = !string.IsNullOrWhiteSpace(requestQuery.Language)
                ? EnumsMapping.ToLanguageEnum(requestQuery.Language)
                : Constants.BaseLanguage;
            var priceRepo = _unitOfWork.GetRepository<Price, Guid>();

            var bookedRepo = _unitOfWork.GetRepository<Booking, Guid>();

            var slotRepo = _unitOfWork.GetRepository<AvailableSlots, Guid>();

            var tourIds = tours.Select(t => t.Id).ToList();

            var prices = await priceRepo.GetAllAsync(
               new GetPricesByTourIdsSpecification(tourIds));
            var pricesByTourId = prices
                    .GroupBy(p => p.TourId)
                    .ToDictionary(g => g.Key, g => g.ToList());
           

            var slots = await slotRepo.GetAllAsync(
                new GetAvailableSlotsByTourIdsSpecification(tourIds));

            var bookedData = await bookedRepo.GetAllAsync(new GetBookingsByTourIdsSpecification(tourIds));

            var travelersCountByTourId = bookedData
                .Where(b => b.OrderItem?.TourId != null)
                .GroupBy(b => b.OrderItem!.TourId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(b => b.Travelers?.Count ?? 0));

            //var tourCapacity = slots
            //  .Where(s => s.AvailableTimeStatus == AvailableDateTimeStatus.Available)
            //  .GroupBy(s => s.TourId)
            //  .ToDictionary(
            //      g => g.Key,
            //      g => g.Sum(x => x.MaxCapacity));


            var mappingTours = tours.Select(tour =>
            {
                var tourPrices = pricesByTourId
                .GetValueOrDefault(tour.Id, []);

                var vipPrice = tourPrices
                    .Where(p => !p.IsDeleted && p.UserType == UserType.VIP &&
                                (p.IsMainActivityType == null || p.IsMainActivityType == true) && p.SpecialDate == null)
                    .OrderByDescending(p => p.RetailPrice)
                    .FirstOrDefault();

                var publicPrice = tourPrices
                    .Where(p => !p.IsDeleted && p.UserType == UserType.Public &&
                                (p.IsMainActivityType == null || p.IsMainActivityType == true) && p.SpecialDate == null)
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

                    //TotalCapacity =
                    //         tourCapacity.GetValueOrDefault(tour.Id, 0),

                    BookedSeats = travelersCountByTourId.GetValueOrDefault(tour.Id, 0),

                    //BookedPercentage = tourCapacity.GetValueOrDefault(tour.Id, 0) > 0
                    //    ? (double)travelersCountByTourId.GetValueOrDefault(tour.Id, 0) / tourCapacity.GetValueOrDefault(tour.Id, 0) * 100
                    //    : 0
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
            if (!BusinessRules.TryCleanGuid(Id, out Guid tourId))
                return Result.Fail("Invalid UUID");

            SupportedLanguage language = Constants.BaseLanguage;
            if (!string.IsNullOrWhiteSpace(requestDTO.Language))
                language = EnumsMapping.ToLanguageEnum(requestDTO.Language);

            var tour = await _unitOfWork
                .GetRepository<Tour, Guid>()
                .GetByIdAsync(new GetTourByIdSpecification(tourId));

            if (tour is null)
                return Result.Fail(new NotFoundError("This Tour Not Found"));

            var tourPrice = await _unitOfWork.GetRepository<Price, Guid>().GetAllAsync(new GetPriceWithTourIdSpecification(tourId));

            var tourCancellations = await _unitOfWork.GetRepository<Cancellation, Guid>().GetAllAsync(new GetCancellationWithTourIdSpecification(tourId));
            var availableTimes = await _unitOfWork.GetRepository<AvailableSlots, Guid>().GetAllAsync(new GetAvailableTimesWithTourIdSpecification(tourId));

            var countryInfo = await _unitOfWork
                .GetRepository<CountryInfo, Guid>()
                .GetByIdAsync(new GetCountryInfoByDestinationIdSpecification(tour.DestinationId, language));
            var inclusions = await _unitOfWork.GetRepository<TourInclusion, Guid>().GetAllAsync(new GetInclustionWithTourIdSpecification(tourId));

            var blackoutDates = await _unitOfWork.GetRepository<BlackoutDate, Guid>().GetAllAsync(new GetBlackoutDatesWithTourIdSpecification(tourId));
            var blackoutWeekDayes = await _unitOfWork.GetRepository<BlackoutWeekDay, Guid>().GetAllAsync(new GetBlackoutWeekDayesSpecification(tourId));


            return Result.Ok(MapTourToResponseDTO(tour, language , countryInfo,tourPrice,availableTimes,tourCancellations, inclusions,blackoutDates,blackoutWeekDayes));
        }


        public async Task<Result> UpdateTourAsync(UpdateTourRequestDTO requestDTO, string Id)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
                return validationResult;


            if (requestDTO.IsFullTime == true && requestDTO.Schedule is not null && requestDTO.Schedule.Any())
            {
                return Result.Fail("You Should Select IsFullTime Or Enter Available Time Not Both");

            }

            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
            var slotRepo = _unitOfWork.GetRepository<AvailableSlots, Guid>();
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

                    if (requestDTO.IsFullTime is not null && requestDTO.IsFullTime == true)
                    {
                        
                        var availableTimes = await slotRepo.GetAllAsync(new GetAllSlotsWithTourIdSpecification(tour.Id));
                        slotRepo.RemoveRange(availableTimes);
                        tour.IsFullTime = true;
                    }
                    if (requestDTO.IsFullTime is not null && requestDTO.IsFullTime == false && requestDTO.Schedule is not null  && requestDTO.Schedule.Any())
                    {
                        tour.IsFullTime = false;
                    }
                    TourMapping.UpdateTour(requestDTO, tour, translation, languageEnum);

                    if (requestDTO.Prices is not null && requestDTO.Prices.Any())
                    {
                        var existingPrices = await _unitOfWork.GetRepository<Price, Guid>().GetAllAsync(new PriceWithoutUserTypeForTourSpecification(tour.Id));
                        await _adminPriceService.UpdatePricesAsync(tour, requestDTO.Prices, languageEnum, existingPrices);

                    }

                    if (requestDTO.Schedule is not null && requestDTO.Schedule.Any())
                    {
                        var schedule = await _unitOfWork.GetRepository<AvailableSlots, Guid>().GetAllAsync(new GetAvailableTimesWithTourIdSpecification(tour.Id));
                        await _adminTourScheduleService.UpdateScheduleAsync(tour, requestDTO.Schedule, schedule);

                    }



                    if ((requestDTO.Includes is not null && requestDTO.Includes.Any()) ||
                        (requestDTO.NotIncludes is not null && requestDTO.NotIncludes.Any()))
                        await _adminTourInclusionService.UpdateInclusionAsync(tour, requestDTO.Includes, requestDTO.NotIncludes, languageEnum);

                    if (requestDTO.Cancellation is not null)
                        await _adminTourCancellationService.UpdateCancellationAsync(tour, requestDTO.Cancellation, languageEnum);

                    if (requestDTO.Images is not null && requestDTO.Images.Any())
                        await _imageService.UpdateImagesAsync(tour, requestDTO.Images);


                    if (requestDTO.BlackoutDates is not null && requestDTO.BlackoutDates.Any())
                    {
                        var blackoutDatesRepo = _unitOfWork.GetRepository<BlackoutDate, Guid>();
                        var blackoutDates = await blackoutDatesRepo.GetAllAsync(new GetBlackoutDatesWithTourIdSpecification(tour.Id));

                        if (blackoutDates.Any())
                        {
                            blackoutDatesRepo.RemoveRange(blackoutDates);
                        }

                        var newBlackoutDates = requestDTO.BlackoutDates.Select(d => new BlackoutDate()
                        {
                            Id = Guid.NewGuid(),
                            Tour = tour,
                            TourId = tour.Id,
                            Date = d.Date

                        });
                        await blackoutDatesRepo.AddRangeAsync(newBlackoutDates);
                    }

                    if (requestDTO.BlackoutWeekDays is not null && requestDTO.BlackoutWeekDays.Any())
                    {
                        var blackoutWeekDaysRepo = _unitOfWork.GetRepository<BlackoutWeekDay, Guid>();
                        var blackoutWeekDays = await blackoutWeekDaysRepo.GetAllAsync(new GetBlackoutWeekDayesSpecification(tour.Id));

                        if (blackoutWeekDays.Any())
                        {
                            blackoutWeekDaysRepo.RemoveRange(blackoutWeekDays);
                        }

                        var newBlackoutWeekDays = requestDTO.BlackoutWeekDays.Select(d => new BlackoutWeekDay()
                        {
                            Id = Guid.NewGuid(),
                            Tour = tour,
                            TourId = tour.Id,
                            DayOfWeek = d.DayOfWeek

                        });
                        await blackoutWeekDaysRepo.AddRangeAsync(newBlackoutWeekDays);
                    }


                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    if (!string.IsNullOrWhiteSpace(requestDTO.Language))
                    {
                        var sourceLanguage = EnumsMapping.ToLanguageEnum(requestDTO.Language);
                        _ = _backgroundTaskQueue.EnqueueAsync(async (serviceProvider, cancellationToken) =>
                        {


                            await TranslateUpdateTour(tour.Id, requestDTO.Title, requestDTO.Description, serviceProvider, sourceLanguage, requestDTO);


                        });
                    }



                    return Result.Ok().WithSuccess(new Success("Tour Updated Successfully"));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error updating tour: {Message}, Inner: {InnerMessage}", ex.Message, ex.InnerException?.Message);
                    return FluentValidationExtension.FromException(details: $"{ex.Message} | Inner: {ex.InnerException?.Message}");
                }
            });
        }

        private async Task TranslateUpdateTour(Guid tourId, string? tourTitle, string? tourDescription, IServiceProvider serviceProvider, SupportedLanguage sourceLanguage, UpdateTourRequestDTO requestDTO)
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

            var inclusions = (await unitOfWork
                .GetRepository<TourInclusion, Guid>()
                .GetAllAsync(new TourInclusionsByTourIdSpec(tourId)))
                .ToList();


            var prices = (await unitOfWork
                .GetRepository<Price, Guid>()
                .GetAllAsync(new PricesByTourIdSpec(tourId)))
                .ToList();

            var inputTranslate = new TourTranslationItem()
            {
                TourId = tourId,
                SourceLanguage = sourceLanguage,
                Title = tourTitle ?? "",
                Description = tourDescription ?? "",
                       
                Inclusions = ((requestDTO.Includes is null && requestDTO.NotIncludes is null) || !inclusions.Any()) ? null :
                            inclusions.Select(i => new InclusionTranslationItem()
                            {
                                InclusionId = i.Id,
                                Text = i.Translations.Where(tr => tr.Language == sourceLanguage).Select(c => c.Text).FirstOrDefault() ?? ""

                            }).ToList()
                ,
                Prices = requestDTO.Prices is null || !prices.Any() ? null :
                prices.Select(p => new PriceTranslationItem()
                {
                    PriceId = p.Id,
                    Type = p.Translations.Where(t => t.Language == sourceLanguage)
                                .Select(t => t.Type)
                                .FirstOrDefault() ?? "",
                    ActivityType = p.Translations.Where(t => t.Language == sourceLanguage)
                                .Select(t => t.ActivityType)
                                .FirstOrDefault() ?? ""

                }).ToList()
            };
            var autoTranslationService =
                           serviceProvider.GetRequiredService<IAutoTranslationService>();

            await autoTranslationService.TranslateTour(
                          sourceLanguage,
                          inputTranslate);
        }


        public GetTourResponseDTO MapTourToResponseDTO(Tour tour, SupportedLanguage language , CountryInfo? country,IEnumerable< Price>? prices ,IEnumerable< AvailableSlots>? tourSchedule ,IEnumerable< Cancellation>? cancellations ,IEnumerable<TourInclusion>? inclustions , IEnumerable<BlackoutDate>? blackoutDates , IEnumerable<BlackoutWeekDay>? blackoutWeekDays)
        {
            var translation = tour.Translations.FirstOrDefault(t => t.Language == language);
            var destinationTranslation = tour.Destination?.Translations.FirstOrDefault(t => t.Language == language);
          
            var countryName = country is null ? null: country.Translations?
                .FirstOrDefault(t => t.Language == language)?
                .Name ?? "";
            return new GetTourResponseDTO(
                Id: tour.Id,
                GuideLanguage: tour.GuideLanguage.ToString() ?? "",
                MeetingPoint: tour.MeetingPoint ?? "",
                Description: translation?.Description ?? "",
                Country: countryName ?? "",
                Images: tour.Images?
                    .Select(i => new GetImageUrlResponseDTO(i.Id, i.ImageUrl))
                    .ToList(),
                Duration: tour.Duration,
                DestinationId: tour.DestinationId,
                Title: translation?.Title ?? "",
                Language: language.ToString(),
                Currency: tour.CurrencyCode.ToString(),
                UserType: tour.UserType.ToString(),

                Cancellation: cancellations? 
                    .Select(c => new GetCancellationResponseDTO(

                        Id: c.Id,
                        CancelationPolicyType: c.CancelationPolicyType.ToString(),
                        CancellationBefore: c.CancellationBefore,
                        RefundPercentage: c.RefundPercentage

                        )).ToList(),
                
                
                Includes: inclustions ?
                    .Where(x => x.IsIncluded)
                    .Select(x => x.Translations.FirstOrDefault(t => t.Language == language)?.Text ?? "")
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList(),

                NotIncludes: inclustions ?
                    .Where(x => !x.IsIncluded)
                    .Select(x => x.Translations.FirstOrDefault(t => t.Language == language)?.Text ?? "")
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList(),

                Prices: prices?
                    .Select(p => new GetPriceResponseDTO(
                        Id: p.Id,
                        Discount: p.Discount,
                        Type: p.Translations.FirstOrDefault(t => t.Language == language)?.Type ?? "",
                        Cost: p.Cost,
                        UserType: p.UserType,
                        IsMainActivityType: p.IsMainActivityType,
                        SpecialDate : p.SpecialDate,
                        ActivityType: p.Translations.FirstOrDefault(t => t.Language == language)?.ActivityType ?? ""))
                    .ToList(),

                BlackoutDates : blackoutDates?
                            .Select(d => new GetBlackoutDateResponseDTO(
                                
                                    Id : d.Id,
                                    Date : d.Date
                                
                                )).ToList(),

                BlackoutWeekDays : blackoutWeekDays?
                            .Select(d => new GetBlackoutWeekDaysResponseDTO(
                                    Id: d.Id,
                                    DayOfWeek : d.DayOfWeek

                                )).ToList(),
                Schedule: tourSchedule?
                    
                            .Select(slot => new GetAvaialbleSlotResponseDTO(
                                slot.Id,
                                slot.StartTime,
                                slot.AvailableTimeStatus.ToString()
                                
                                
                                ))
                            
                            
                            .ToList(),



                IsFullTime: tour.IsFullTime,

                IsPitsAllowed: tour.IsPitsAllowed,
                IsWheelchairAvailable: tour.IsWheelchairAvailable
            );
        }

      
        //public async Task<Result<object>> GetActivityStatsAsync()
        //{
        //    var now = DateTime.UtcNow;
        //    var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        //    var nextMonthStart = monthStart.AddMonths(1);

        //    var bookingRepo = _unitOfWork.GetRepository<Booking, Guid>();
        //    var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        //    var slotRepo = _unitOfWork.GetRepository<AvailableSlots, Guid>();
        //    var reservationRepo = _unitOfWork.GetRepository<SlotReservation, Guid>();

        //    var bookingsThisMonth = await bookingRepo.GetAllAsync(new GetBookingsByDateSpecification(monthStart, nextMonthStart));
        //    var bookingsCount = bookingsThisMonth.Count();

        //    var orders = await orderRepo.GetAllAsync(new GetOrdersByDateAndStatusSpecification(monthStart, nextMonthStart, OrderStatus.Paid));
        //    var grossRevenue = orders.Sum(o => o.TotalAmount);

        //    var slotsThisMonth = await slotRepo.GetAllAsync(new GetSlotsByDateSpecification(monthStart, nextMonthStart));
        //    var totalCapacity = slotsThisMonth.Sum(s => s.MaxCapacity);

        //    var confirmedReservationsThisMonth = await reservationRepo.GetAllAsync(new GetConfirmedReservationsByDateSpecification(monthStart, nextMonthStart));
        //    var totalBookedSeats = confirmedReservationsThisMonth.Sum(r => r.Quantity);

        //    var avgCapacity = totalCapacity <= 0
        //        ? 0
        //        : Math.Clamp((int)Math.Round((decimal)totalBookedSeats * 100m / totalCapacity, MidpointRounding.AwayFromZero), 0, 100);

        //    var status = avgCapacity >= 90 ? "Low Stock" : "Active";

        //    var dailyRevenue = new decimal[DateTime.DaysInMonth(now.Year, now.Month)];
        //    foreach (var order in orders)
        //        if (order.OrderDate.HasValue)
        //            dailyRevenue[order.OrderDate.Value.Day - 1] += order.TotalAmount;

        //    var bookingsPerDestination = bookingsThisMonth
        //        .Where(b => b.OrderItem != null && b.OrderItem.DestinationName != null)
        //        .GroupBy(b => b.OrderItem.DestinationName)
        //        .Select(g => new { Label = g.Key, Count = g.Count() })
        //        .OrderByDescending(x => x.Count)
        //        .Take(4)
        //        .ToList();

        //    int totalBookings = bookingsCount > 0 ? bookingsCount : 1;

        //    var regionalMix = bookingsPerDestination
        //        .Select(x => new
        //        {
        //            x.Label,
        //            Pct = (int)Math.Round((decimal)x.Count * 100m / totalBookings),
        //            x.Count
        //        })
        //        .ToList<object>();

        //    if (!regionalMix.Any())
        //        regionalMix.Add(new { Label = "Global", Pct = 100, Count = 0 });

        //    return Result.Ok<object>(new
        //    {
        //        bookingsThisMonth = bookingsCount,
        //        avgCapacity,
        //        grossRevenue,
        //        status,
        //        dailyRevenue,
        //        regionalMix
        //    });
        //}

        
    }
}
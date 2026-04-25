using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Application.Specifications.TourSpecification;
using Amigo.Application.Specifications.TourSpecification.User;
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
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.Admin
{
    public class AdminTourService (IValidationService _validationService,
                                    IUnitOfWork _unitOfWork,
                                    ITourMapping _tourMapping,
                                    IImageMapping _imageMapping,
                                    IPriceMapping _priceMapping,
                                    ITourScheduleMapping _tourScheduleMapping,
                                    IInclusionMapping _inclusionMapping,
                                    ICancellationMapping _cancellationMapping,
                                    IAdminPriceService _adminPriceService,
                                    IAdminTourScheduleService _adminTourScheduleService,
                                    IAdminTourInclusionService _adminTourInclusionService,
                                    IAdminTourCancellationService _adminTourCancellationService,
                                    IImageService _imageService
                                    ) 
                                : IAdminTourService
    {
        public async Task<Result<CreateTourResponseDTO>> CreateTourAsync(CreateTourRequestDTO requestDTO)
        {  
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
           

            var destination = await _unitOfWork.GetRepository<Destination, Guid>().GetByIdAsync(new GetNotDeletedDestinationByIdSpecification( requestDTO.DestinationId));
            if (destination is null)
            {
                return Result.Fail(new NotFoundError("This Destination Not Found"));

            }

            if (requestDTO.Prices is not null && requestDTO.Prices.Any(p =>
                    p.UserType == UserType.None
                    || (p.UserType & requestDTO.UserType) != p.UserType))
            {
                return Result.Fail("Invalid pricing tiers: tier user type must match selected tour audience.");
            }

            var tour = _tourMapping.TourToEntity(requestDTO, destination);

            var tourTranslation = _tourMapping.TourTranslationToEntity(requestDTO, tour);

            var tourImages = requestDTO.Images is not null && requestDTO.Images.Any()
                                ? _imageMapping.ImagesToEntity(requestDTO.Images, tour).ToList()
                                : new List<TourImage>();

             
             var tourPrices = requestDTO.Prices is not null && requestDTO.Prices.Any()
                              ? _priceMapping.PricesDTOToEntity(requestDTO.Prices, tour,requestDTO.Language)
                              :new List<Price>();
            
            var tourSchedule = requestDTO.Schedule is not null && requestDTO.Schedule.Any()
                                ?_tourScheduleMapping.TourSchedulesDTOToEntity(requestDTO.Schedule, tour)
                                :new List<TourSchedule>();
                
            var tourInclusion = (requestDTO.Includes is not null && requestDTO.Includes.Any()) || (requestDTO.NotIncludes is not null && requestDTO.NotIncludes.Any())
                                ? _inclusionMapping.TourInclusionToEntity(requestDTO.Includes,requestDTO.NotIncludes ,tour, requestDTO.Language)
                                :new List<TourInclusion>();

           
            var cancellation = requestDTO.Cancellation is not null ?
                                    _cancellationMapping.CancellationToEntity(requestDTO.Cancellation, tour,requestDTO.Language)
                                    :new Cancellation();

           


            var strategy = _unitOfWork.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    await _unitOfWork.GetRepository<Tour, Guid>().AddAsync(tour);
                    await _unitOfWork.GetRepository<TourTranslation, Guid>().AddAsync(tourTranslation);
                    if (tourImages.Any())
                    {
                        await _unitOfWork.GetRepository<TourImage, Guid>().AddRangeAsync(tourImages);
                    }
                    if (tourPrices.Any())
                    {
                        await _unitOfWork.GetRepository<Price, Guid>().AddRangeAsync(tourPrices);
                    }

                    if (tourSchedule.Any())
                    {
                        await _unitOfWork.GetRepository<TourSchedule, Guid>().AddRangeAsync(tourSchedule);

                    }

                    if ((tourInclusion is not null && tourInclusion.Any()))
                    {
                        await _unitOfWork.GetRepository<TourInclusion, Guid>().AddRangeAsync(tourInclusion);
                    }

                    if (cancellation is not null)
                    {
                        await _unitOfWork.GetRepository<Cancellation, Guid>().AddAsync(cancellation);
                    }
                    

                    await _unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Result.Ok( new CreateTourResponseDTO(tour.Id) )
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

        public async Task<Result<PaginatedResponse<AdminTourListItemResponseDTO>>> GetAllToursAsync(GetAllAdminTourQuery requestQuery)
        {
            var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
            var tours = await tourRepo.GetAllAsync(new GetAllToursForAdminSpecification(requestQuery));
            Language language = EnumsMapping.ToLanguageEnum(requestQuery.Language);
            var bookedRepo = _unitOfWork.GetRepository<Booking, Guid>();
            var tourIds = tours.Select(t => t.Id).ToList();
            var bookedData = await bookedRepo.GetAllAsync(new GetBookingsByTourIdsSpecification(tourIds));
            var bookedSeatsData = bookedData.Sum(b => b.RequiredTravelersCount);

            var MappingTours = tours.Select(tour =>
            {
                var vipPrice = tour.Prices
                    .FirstOrDefault(p => !p.IsDeleted && p.UserType == UserType.VIP);

                var publicPrice = tour.Prices
                    .FirstOrDefault(p => !p.IsDeleted && p.UserType == UserType.Public);

                var translation = tour.Translations
                    .FirstOrDefault(t => t.Language == language);

                var destinationTranslation = tour.Destination.Translations
                    .FirstOrDefault(t => t.Language == language);

                return new AdminTourListItemResponseDTO()
                {
                    TourId = tour.Id,

                    Title = translation?.Title ?? "",

                    DestinationName = destinationTranslation?.Name ?? "",

                    ImageUrl = tour.Images.FirstOrDefault()?.ImageUrl,

                    EntryAmountVIP = vipPrice?.RetailPrice ?? 0,

                    EntryAmountVIPLabel =
                        vipPrice?.Translations
                            .FirstOrDefault(t => t.Language == language)?
                            .Type ?? "",

                    EntryAmountPublic = publicPrice?.RetailPrice ?? 0,

                    EntryAmountPublicLabel =
                        publicPrice?.Translations
                            .FirstOrDefault(t => t.Language == language)?
                            .Type ?? "",

                    TotalCapacity = tour.AvailableTimes
                        .SelectMany(t => t.AvailableSlots)
                        .Where(s => s.AvailableTimeStatus == AvailableDateTimeStatus.Available)
                        .Sum(s => s.MaxCapacity),

                    BookedSeats = bookedSeatsData

                };
            });
            var countTourData = await tourRepo.GetCountSpecificationAsync(new CountGetAllTourForAdminSpecification(requestQuery));
            var response =  new PaginatedResponse<AdminTourListItemResponseDTO>
            {
                Data = MappingTours,

                PageNumber = requestQuery.PageNumber,
                PageSize = requestQuery.PageSize,
                TotalItems = countTourData
            };
            return Result.Ok(response);
        }

        public async Task<Result<GetTourResponseDTO>> GetTourById(string Id, GetTourByIdRequestDTO requestDTO)
        {
            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid tourId = guid;

            Language language = Language.en;
            if (!string.IsNullOrWhiteSpace(requestDTO.Language)) language = EnumsMapping.ToLanguageEnum(requestDTO.Language);



            var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
            var tour = await tourRepo.GetByIdAsync(new GetTourByIdSpecification(tourId));
            if (tour is null)
            {
                return Result.Fail(new NotFoundError("This Tour Not Found"));

            }
            var mappedTour = MapTourToResponseDTO(tour, language);
            return Result.Ok(mappedTour);
        }

        public async Task<Result> UpdateTourAsync(UpdateTourRequestDTO requestDTO, string Id)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid tourId = guid;


            var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
            var strategy = _unitOfWork.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var tour = await tourRepo.GetByIdAsync(new GetTourByIdSpecification(tourId));
                    if (tour is null)
                    {
                        return Result.Fail(new NotFoundError("This Tour Not Found"));

                    }
                    TourTranslation? translation = null;
                    Language? languageEnum = null;

                    if (!string.IsNullOrWhiteSpace(requestDTO.Language))
                    {
                        languageEnum = EnumsMapping.ToLanguageEnum(requestDTO.Language);
                    }


                    if (requestDTO.Language is not null)
                    {
                        ;
                        translation = tour.Translations
                                     .FirstOrDefault(t => t.Language == languageEnum);
                    }
                    /*
                       Handel if this tour has booked :

                     */

                    if (requestDTO.DestinationId is not null)
                    {
                        var destination = await _unitOfWork.GetRepository<Destination, Guid>().GetByIdAsync(new GetNotDeletedDestinationByIdSpecification(requestDTO.DestinationId.Value));

                        if (destination is null)
                        {
                            return Result.Fail(new NotFoundError("This Destination Not Found"));

                        }
                        tour.Destination = destination;
                        tour.DestinationId = destination.Id;
                    }
                    _tourMapping.UpdateTour(requestDTO, tour, translation, languageEnum);

                    if (requestDTO.Prices is not null && requestDTO.Prices.Any())
                        await _adminPriceService.UpdatePricesAsync(tour, requestDTO.Prices, languageEnum);

                    if (requestDTO.Schedule is not null && requestDTO.Schedule.Any())
                        await _adminTourScheduleService.UpdateScheduleAsync(tour, requestDTO.Schedule);


                    if ((requestDTO.Includes is not null && requestDTO.Includes.Any()) || (requestDTO.NotIncludes is not null && requestDTO.NotIncludes.Any()))
                        await _adminTourInclusionService.UpdateInclusionAsync(tour, requestDTO.Includes,requestDTO.NotIncludes, languageEnum);

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
        public GetTourResponseDTO MapTourToResponseDTO(Tour tour, Language language)
        {
            var translation = tour.Translations
                .FirstOrDefault(t => t.Language == language);

            var destinationTranslation = tour.Destination?.Translations
                .FirstOrDefault(t => t.Language == language);

            var cancellation = tour.Cancellation;

            return new GetTourResponseDTO
            (
                Id: tour.Id,
                GuideLanguage: tour.GuideLanguage.ToString() ?? "",

                MeetingPoint: tour.MeetingPoint,


                Images: tour.Images?
                    .Select(i => new GetImageUrlResponseDTO(i.Id, i.ImageUrl))
                    .ToList(),

                Duration: tour.Duration,

                DestinationId: tour.DestinationId,

                Title : translation?.Title ?? "",

                Description : translation?.Description ?? "",   

                Language: language.ToString(),

                Currency: tour.CurrencyCode.ToString(),

                UserType: tour.UserType.ToString(),

                            Cancellation: cancellation == null
                ? null
                : new GetCancellationResponseDTO
                (
                    Id: cancellation.Id,
                    CancelationPolicyType: cancellation.CancelationPolicyType.ToString(),
                    CancellationBefore: cancellation.CancellationBefore,
                    RefundPercentage: cancellation.RefundPercentage,

                    Description: cancellation.Translations
                        .FirstOrDefault(t => t.Language == language)?
                        .Description ?? ""
                ),

                Includes: tour.TourInclusions
                    .Where(x => x.IsIncluded)
                    .Select(x =>
                         x.Translations.FirstOrDefault(t => t.Language == language)?
                                                                    .Text ?? ""
                    )
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList(),

                NotIncludes: tour.TourInclusions
                    .Where(x => !x.IsIncluded)
                    .Select(x =>
                         x.Translations.FirstOrDefault(t => t.Language == language)?
                                             .Text ?? ""
                    )
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList(),

                Prices: tour.Prices?
                    .Select(p => new GetPriceResponseDTO
                    (
                        Id: p.Id,
                        Discount: p.Discount,

                        Type: p.Translations
                            .FirstOrDefault(t => t.Language == language)?
                            .Type ?? "",

                        Cost: p.Cost,
                        UserType: p.UserType
                    ))
                    .ToList(),

                Schedule: tour.AvailableTimes?
                    .Select(s => new GetTourScheduleResponseDTO
                    (
                        Id: s.Id,
                        AvailableDateStatus: s.AvailableDateStatus.ToString(),
                        StartDate: s.StartDate,

                        availableSlots: s.AvailableSlots?
                            .Select(slot => new GetAvaialbleSlotResponseDTO
                            (
                                slot.Id,
                                slot.StartTime,
                                
                                slot.AvailableTimeStatus.ToString(),
                                slot.MaxCapacity,
                                slot.ReservedCount
                            ))
                            .ToList()
                    ))
                    .ToList(),

                IsPitsAllowed: tour.IsPitsAllowed,
                IsWheelchairAvailable: tour.IsWheelchairAvailable
            );
        }
    }
}

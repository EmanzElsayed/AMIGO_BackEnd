using Amigo.Application.Specifications.TourSpecification;
using Amigo.Domain.DTO.Tour;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.SharedKernal.DTOs.Tour;
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
                                    INotIncludedMapping _notIncludedMapping,
                                    IIncludeMapping _includeMapping,
                                    ICancellationMapping _cancellationMapping,
                                    IAdminPriceService _adminPriceService,
                                    IAdminTourScheduleService _adminTourScheduleService,
                                    IAdminTourNotIncludesService _adminTourNotIncludesService,
                                    IAdminTourIncludesService _adminTourIncludesService,
                                    IAdminTourCancellationService _adminTourCancellationService,
                                    IImageService _imageService) 
                                : IAdminTourService
    {
        public async Task<Result<CreateTourResponseDTO>> CreateTourAsync(CreateTourRequestDTO requestDTO)
        {  
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
           

            var destination = await _unitOfWork.GetRepository<Destination, Guid>().GetByIdAsync(requestDTO.DestinationId);
            if (destination is null)
            {
                return Result.Fail(new NotFoundError("This Destination Not Found"));

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
                
            var tourIncludes = requestDTO.Includes is not null && requestDTO.Includes.Any()
                                ? _includeMapping.TourIncludesToEntity(requestDTO.Includes, tour, requestDTO.Language)
                                :new List<TourIncluded>();

            var tourNotIncludes = requestDTO.NotIncludes is not null && requestDTO.NotIncludes.Any()
                                  ? _notIncludedMapping.TourNotIncludesToEntity(requestDTO.NotIncludes, tour, requestDTO.Language)
                                  :new List<TourNotIncluded>();

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

                    if (tourIncludes is not null && tourIncludes.Any())
                    {
                        await _unitOfWork.GetRepository<TourIncluded, Guid>().AddRangeAsync(tourIncludes);
                    }

                    if (tourNotIncludes is not null && tourNotIncludes.Any())
                    {
                        await _unitOfWork.GetRepository<TourNotIncluded, Guid>().AddRangeAsync(tourNotIncludes);
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

                    if (requestDTO.NotIncludes is not null && requestDTO.NotIncludes.Any())
                        await _adminTourNotIncludesService.UpdateExcludesAsync(tour, requestDTO.NotIncludes, languageEnum);

                    if (requestDTO.Includes is not null && requestDTO.Includes.Any())
                        await _adminTourIncludesService.UpdateIncludesAsync(tour, requestDTO.Includes, languageEnum);

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

        //public async Task<Result> UpdateTourAsync(UpdateTourRequestDTO requestDTO, Guid tourId)
        //{
        //    var validationResult = await _validationService.ValidateAsync(requestDTO);
        //    if (!validationResult.IsSuccess)
        //        return validationResult;

        //    var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();
        //    var bookingRepo = _unitOfWork.GetRepository<Booking, Guid>();

        //    var tour = await tourRepo.GetByIdAsync(
        //        new TourWithFullDetailsSpecification(tourId)
        //    );

        //    if (tour is null)
        //        return Result.Fail(new NotFoundError("This Tour Not Found"));

        //    // 🔥 check active bookings (critical business rule)
        //    var hasActiveBookings = await bookingRepo.AnyAsync(
        //        new ActiveBookingsForTourSpecification(tourId)
        //    );

        //    // ======================================
        //    // 🚨 RESTRICTED UPDATES IF BOOKED
        //    // ======================================
        //    if (hasActiveBookings)
        //    {
        //        if (requestDTO.DestinationId is not null ||
        //            requestDTO.Schedule is not null ||
        //            requestDTO.Prices is not null ||
        //            requestDTO.Cancellation is not null)
        //        {
        //            return Result.Fail(
        //                new ConfilctError("Cannot modify Destination, Schedule, Prices or Cancellation because tour has active bookings")
        //            );
        //        }
        //    }

        //    // ======================================
        //    // 🔥 Destination update (safe)
        //    // ======================================
        //    if (requestDTO.DestinationId is not null)
        //    {
        //        var destination = await _unitOfWork
        //            .GetRepository<Destination, Guid>()
        //            .GetByIdAsync(requestDTO.DestinationId.Value);

        //        if (destination is null)
        //            return Result.Fail(new NotFoundError("This Destination Not Found"));

        //        tour.Destination = destination;
        //        tour.DestinationId = destination.Id;
        //    }

        //    // ======================================
        //    // 🔥 Basic fields
        //    // ======================================
        //    if (requestDTO.Duration is not null)
        //        tour.Duration = requestDTO.Duration.Value;

        //    if (requestDTO.MeetingPoint is not null)
        //        tour.MeetingPoint = requestDTO.MeetingPoint;

        //    if (requestDTO.GuideLanguage is not null)
        //        tour.GuideLanguage = requestDTO.GuideLanguage;

        //    if (requestDTO.UserType is not null)
        //        tour.UserType = requestDTO.UserType.Value;

        //    if (requestDTO.Currency is not null)
        //        tour.CurrencyCode = EnumsMapping.ToEnum<Currency>(requestDTO.Currency, false);

        //    if (requestDTO.IsPitsAllowed is not null)
        //        tour.IsPitsAllowed = requestDTO.IsPitsAllowed.Value;

        //    if (requestDTO.IsWheelchairAvailable is not null)
        //        tour.IsWheelchairAvailable = requestDTO.IsWheelchairAvailable.Value;

        //    // ======================================
        //    // 🔥 Translation (same logic as Destination)
        //    // ======================================
        //    if (requestDTO.Language is not null)
        //    {
        //        var language = EnumsMapping.ToLanguageEnum(requestDTO.Language);

        //        var translation = tour.Translations
        //            .FirstOrDefault(x => x.Language == language);

        //        if (!string.IsNullOrWhiteSpace(requestDTO.Title) ||
        //            !string.IsNullOrWhiteSpace(requestDTO.Description))
        //        {
        //            if (translation is null)
        //            {
        //                tour.Translations.Add(new TourTranslation
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Language = language,
        //                    Title = requestDTO.Title,
        //                    Description = requestDTO.Description,
        //                    TourId = tour.Id
        //                });
        //            }
        //            else
        //            {
        //                if (requestDTO.Title is not null)
        //                    translation.Title = requestDTO.Title;

        //                if (requestDTO.Description is not null)
        //                    translation.Description = requestDTO.Description;
        //            }
        //        }

        //        // ======================================
        //        // Includes (soft replace per language)
        //        // ======================================
        //        if (requestDTO.Includes is not null)
        //        {
        //            tour.Included.RemoveAll(x => x.Language == language);

        //            tour.Included.AddRange(
        //                requestDTO.Includes.Select(i => new TourIncluded
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Language = language,
        //                    Included = i,
        //                    TourId = tour.Id
        //                })
        //            );
        //        }

        //        // Not Includes
        //        if (requestDTO.NotIncludes is not null)
        //        {
        //            tour.NotIncludes.RemoveAll(x => x.Language == language);

        //            tour.NotIncludes.AddRange(
        //                requestDTO.NotIncludes.Select(i => new TourNotIncluded
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Language = language,
        //                    NotIncluded = i,
        //                    TourId = tour.Id
        //                })
        //            );
        //        }

        //        // ======================================
        //        // Prices (ONLY if no active bookings)
        //        // ======================================
        //        if (!hasActiveBookings && requestDTO.Prices is not null)
        //        {
        //            tour.Prices.Clear();

        //            foreach (var price in requestDTO.Prices)
        //            {
        //                tour.Prices.Add(new Price
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Cost = price.Cost ?? 0,
        //                    Discount = price.Discount ?? 0,
        //                    UserType = price.UserType ?? UserType.Public,
        //                    TourId = tour.Id
        //                });
        //            }
        //        }
        //    }

        //    // ======================================
        //    // 🔥 Save
        //    // ======================================
        //    try
        //    {
        //        await _unitOfWork.SaveChangesAsync();

        //        return Result.Ok()
        //            .WithSuccess(new Success("Tour Updated Successfully"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return FluentValidationExtension.FromException(ex.Message);
        //    }
        //}
    }
}

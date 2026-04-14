using Amigo.Application.Specifications.TourSpecification;
using Amigo.Domain.DTO.Tour;
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
                                    IInclusionMapping _inclusionMapping,
                                    ICancellationMapping _cancellationMapping,
                                    IAdminPriceService _adminPriceService,
                                    IAdminTourScheduleService _adminTourScheduleService,
                                    IAdminTourInclusionService _adminTourInclusionService,
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
    }
}

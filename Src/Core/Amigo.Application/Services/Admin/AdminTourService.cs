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
                                    INotIncludedMapping _notIncludedMapping,
                                    IIncludeMapping _includeMapping,
                                    ICancellationMapping _cancellationMapping) 
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
                
            var tourIncludes = requestDTO.Includes is not null && requestDTO.Includes.Any()
                                ? _includeMapping.TourIncludesToEntity(requestDTO.Includes, tour, requestDTO.Language)
                                :new List<TourIncluded>();

            var tourNotIncludes = requestDTO.NotIncludes is not null && requestDTO.NotIncludes.Any()
                                  ? _notIncludedMapping.TourNotIncludesToEntity(requestDTO.NotIncludes, tour, requestDTO.Language)
                                  :new List<TourNotIncluded>();

            var cancellation = requestDTO.Cancellation is not null ?
                                    _cancellationMapping.CancellationToEntity(requestDTO.Cancellation, tour,requestDTO.Language)
                                    :new Cancellation();

            //var cancellationTranslation = requestDTO.Cancellation is not null && string.IsNullOrWhiteSpace(requestDTO.Cancellation.Description) ?
            //                            _cancellationMapping.CancellationTranslationToEntity(requestDTO.Cancellation, cancellation, requestDTO.Language)
            //                            :new CancellationTranslation();


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
                    //if (cancellationTranslation is not null)
                    //{ 
                    //    await _unitOfWork.GetRepository<CancellationTranslation,Guid>().AddAsync(cancellationTranslation);
                    //}

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
    }
}

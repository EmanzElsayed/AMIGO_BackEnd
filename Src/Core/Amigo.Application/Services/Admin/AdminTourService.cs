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
                                    IIncludeMapping _includeMapping) 
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

            var tourPrices = _priceMapping.PricesDTOToEntity(requestDTO.Prices, tour,requestDTO.Language);

            var tourSchedule = _tourScheduleMapping.TourSchedulesDTOToEntity(requestDTO.Schedule, tour);

            var tourIncludes = _includeMapping.TourIncludesToEntity(requestDTO.Includes, tour, requestDTO.Language);

            var tourNotIncludes = _notIncludedMapping.TourNotIncludesToEntity(requestDTO.NotIncludes, tour, requestDTO.Language);



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
                        await _unitOfWork.GetRepository<TourSchedule,Guid>().AddRangeAsync(tourSchedule);

                    }

                    if (tourIncludes is not null && tourIncludes.Any())
                    {
                        await _unitOfWork.GetRepository<TourIncluded, Guid>().AddRangeAsync(tourIncludes);
                    }

                    if (tourNotIncludes is not null && tourNotIncludes.Any())
                    {
                        await _unitOfWork.GetRepository<TourNotIncluded, Guid>().AddRangeAsync(tourNotIncludes);
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
    }
}

using Amigo.Application.Abstraction.MappingInterfaces;
using Amigo.Application.Mapping;
using Amigo.Application.Specifications.Destination;
using Amigo.Domain.Abstraction;
using Amigo.Domain.DTO.Destination;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.DTOs.Destination;
using Amigo.SharedKernal.DTOs.Results;
using Amigo.SharedKernal.QueryParams;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class DestinationService(IValidationService _validationService,
                                    IUnitOfWork _unitOfWork ,
                                    IDestinationMapping _destinationMapping) : IDestinationService
    {
        public async Task<Result> CreateDestinationAsync(CreateDestinationRequestDTO requestDTO)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
            var destination = _destinationMapping.DestinationToEntity(requestDTO);

            var destinationTranslation = _destinationMapping.DestinationTranslationToEntity(requestDTO,destination);

            destination.Translations.Add(destinationTranslation);


            var strategy = _unitOfWork.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    await _unitOfWork.GetRepository<Destination, Guid>().AddAsync(destination);
                    await _unitOfWork.GetRepository<DestinationTranslation, Guid>().AddAsync(destinationTranslation);
                    await _unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Result.Ok()
                                 .WithSuccess(new Success("Destination Created Successfully")
                                 .WithMetadata("StatusCode", (int)HttpStatusCode.Created));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return FluentValidationExtension.FromException(details: ex.Message);
                }
            });
        }

        public async Task<Result<PaginatedResponse<GetTranslationDestinationResponseDTO>>> GetAllDestinationAsync(GetAllDestinationQuery requestQuery)
        {
            var validationResult = await _validationService.ValidateAsync(requestQuery);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
            var destinationRepo = _unitOfWork.GetRepository<DestinationTranslation, Guid>();
            var destinationSpecification = new GetAllDestinationSpecification(requestQuery);
            var destinationData = await destinationRepo.GetAllAsync(destinationSpecification);

            var countDestinationSpecification = new CountGetAllDestinationSpecification(requestQuery);
            var countDestinationData = await destinationRepo.GetCountSpecificationAsync(countDestinationSpecification);

            var mappedDestinationData = _destinationMapping.EntityToDestination(destinationData);
            var paginatedResult = new PaginatedResponse<GetTranslationDestinationResponseDTO>
            {
                Data = mappedDestinationData,
                PageNumber = requestQuery.PageNumber,
                PageSize = requestQuery.PageSize,
                TotalItems = countDestinationData
            };
            return Result.Ok(paginatedResult);
        }
    }
}

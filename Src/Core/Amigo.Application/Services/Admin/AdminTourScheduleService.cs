using Amigo.Domain.DTO.TourSchedule;
using Amigo.Domain.Entities;
using Amigo.SharedKernal.DTOs.Price;
using Amigo.SharedKernal.DTOs.TourSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.Admin
{
    public class AdminTourScheduleService(IValidationService _validationService,
                                    IUnitOfWork _unitOfWork,
                                    ITourScheduleMapping _tourScheduleMapping) : IAdminTourScheduleService
    {
        public async Task<Result<CreateTourScheduleResponseDTO>> CreateTourScheduleAsync(CreateTourScheduleRequestDTO requestDTO)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
            var tour = await _unitOfWork.GetRepository<Tour, Guid>().GetByIdAsync(requestDTO.TourId);
            if (tour is null)
            {
                return Result.Fail(new NotFoundError("This Tour Not Found"));

            }
            var tourSchedule = _tourScheduleMapping.TourScheduleDTOToEntity(requestDTO, tour);
            try
            {
                await _unitOfWork.GetRepository<TourSchedule, Guid>().AddAsync(tourSchedule);
                await _unitOfWork.SaveChangesAsync();

                return Result.Ok(new CreateTourScheduleResponseDTO(tourSchedule.Id))
                            .WithSuccess(new Success("Schedule Created Successfully")
                            .WithMetadata("StatusCode", (int)HttpStatusCode.Created));
            }
            catch (Exception ex)
            {

                return FluentValidationExtension.FromException(details: ex.Message);

            }

        }
    }
}

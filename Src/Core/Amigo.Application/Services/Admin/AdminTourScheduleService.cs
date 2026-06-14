using Amigo.Domain.DTO.AvailableSlots;
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

                                     IAdminAvailableSlotsService _adminAvailableSlotsService)
                            : IAdminTourScheduleService
    {
        //public async Task<Result<CreateTourScheduleResponseDTO>> CreateTourScheduleAsync(CreateTourScheduleRequestDTO requestDTO)
        //{
        //    var validationResult = await _validationService.ValidateAsync(requestDTO);
        //    if (!validationResult.IsSuccess)
        //    {
        //        return validationResult;
        //    }
        //    var tour = await _unitOfWork.GetRepository<Tour, Guid>().GetByIdAsync(requestDTO.TourId);
        //    if (tour is null)
        //    {
        //        return Result.Fail(new NotFoundError("This Tour Not Found"));

        //    }
        //    var tourSchedule = _tourScheduleMapping.TourScheduleDTOToEntity(requestDTO, tour);
        //    try
        //    {
        //        await _unitOfWork.GetRepository<TourSchedule, Guid>().AddAsync(tourSchedule);
        //        await _unitOfWork.SaveChangesAsync();

        //        return Result.Ok(new CreateTourScheduleResponseDTO(tourSchedule.Id))
        //                    .WithSuccess(new Success("Schedule Created Successfully")
        //                    .WithMetadata("StatusCode", (int)HttpStatusCode.Created));
        //    }
        //    catch (Exception ex)
        //    {

        //        return FluentValidationExtension.FromException(details: ex.Message);

        //    }

        //}
        public async Task UpdateScheduleAsync(
                    Tour tour,
                    List<UpdateAvailableSlotsRequestDTO> schedulesDto,
                    IEnumerable<AvailableSlots>? existingSchedules)
        {
            if (schedulesDto is null || schedulesDto.Count == 0)
                return ;

            //  تحسين الأداء

            var slotRepo = _unitOfWork.GetRepository<AvailableSlots, Guid>();

            if (existingSchedules is not null && existingSchedules.Any())
            {
                slotRepo.RemoveRange(existingSchedules);
            }
            List<AvailableSlots> slotList = new List<AvailableSlots>();
            foreach (var scheduleDto in schedulesDto)
            {
                
                //  Create slots
                var newSchedule = new AvailableSlots
                {
                    Id = Guid.NewGuid(),
                    StartTime = scheduleDto.Time ?? default,
                    Tour = tour,
                    TourId = tour.Id,
                };

                
               slotList.Add(newSchedule);
              
            }
            await slotRepo.AddRangeAsync(slotList);
        }
        
    }
}

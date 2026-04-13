using Amigo.Application.Abstraction.Services.Admin;
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
                                    ITourScheduleMapping _tourScheduleMapping,
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
        public Task UpdateScheduleAsync(
                    Tour tour,
                    List<UpdateTourScheduleRequestDTO> schedulesDto)
        {
            if (schedulesDto is null || schedulesDto.Count == 0)
                return Task.CompletedTask;

            //  تحسين الأداء
            var existingSchedulesDict = tour.AvailableTimes
                .Where(s => s.Id != Guid.Empty)
                .ToDictionary(s => s.Id, s => s);

            foreach (var scheduleDto in schedulesDto)
            {
                if (scheduleDto.Id is not null &&
                    existingSchedulesDict.TryGetValue(scheduleDto.Id.Value, out var existingSchedule))
                {
                    // Update Schedule
                    if (scheduleDto.StartDate is not null)
                        existingSchedule.StartDate = scheduleDto.StartDate.Value;

                    if (!string.IsNullOrWhiteSpace(scheduleDto.AvailableDateStatus))
                        existingSchedule.AvailableDateStatus = EnumsMapping.ToAvailableSheduleStatus(scheduleDto.AvailableDateStatus);

                    //  Update Slots
                   _adminAvailableSlotsService.UpdateAvailableSlots(existingSchedule, scheduleDto.availableSlots);
                }
                else
                {
                    //  Create Schedule
                    var newSchedule = new TourSchedule
                    {
                        StartDate = scheduleDto.StartDate ?? default,
                        AvailableDateStatus = EnumsMapping.ToAvailableSheduleStatus(scheduleDto.AvailableDateStatus),
                        TourId = tour.Id,
                        AvailableSlots = new List<AvailableSlots>()
                    };

                    //  Add Slots
                    _adminAvailableSlotsService.AddAvailableSlots(newSchedule, scheduleDto.availableSlots);

                    tour.AvailableTimes.Add(newSchedule);
                }
            }

            return Task.CompletedTask;
        }
    }
}

using Amigo.Domain.DTO.AvailableSlots;
using Amigo.Domain.Entities;
using Amigo.SharedKernal.DTOs.TourSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.Admin
{
    public class AdminAvailableSlotsService(IValidationService _validationService,
                                    IUnitOfWork _unitOfWork,
                                    IAvailableSlotsMapping _availableSlotsMapping) 
                                    : IAdminAvailableSlotsService
    {
        //public async Task<Result> CreateAvailableSlotsAsync(CreateAvailableSlotsRequestDTO requestDTO)
        //{
        //    var validationResult = await _validationService.ValidateAsync(requestDTO);
        //    if (!validationResult.IsSuccess)
        //    {
        //        return validationResult;
        //    }
        //    var tourSchedule = await _unitOfWork.GetRepository<TourSchedule, Guid>().GetByIdAsync(requestDTO.TourScheduleId);
        //    if (tourSchedule is null)
        //    {
        //        return Result.Fail(new NotFoundError("This Tour Schedule Not Found"));

        //    }
        //    var availableSlots = _availableSlotsMapping.AvailableSlotsDTOToEntity(requestDTO, tourSchedule);
        //    try
        //    {
        //        await _unitOfWork.GetRepository<AvailableSlots, Guid>().AddAsync(availableSlots);
        //        await _unitOfWork.SaveChangesAsync();

        //        return Result.Ok()
        //                    .WithSuccess(new Success("Available Slots Created Successfully")
        //                    .WithMetadata("StatusCode", (int)HttpStatusCode.Created));
        //    }
        //    catch (Exception ex)
        //    {

        //        return FluentValidationExtension.FromException(details: ex.Message);

        //    }
        //}
        public void UpdateAvailableSlots(
            TourSchedule schedule,
            List<UpdateAvailableSlotsRequestDTO>? slotsDto)
        {
            if (slotsDto is null || slotsDto.Count == 0)
                return;

            var existingSlotsDict = schedule.AvailableSlots
                .Where(s => s.Id != Guid.Empty)
                .ToDictionary(s => s.Id, s => s);

            foreach (var slotDto in slotsDto)
            {
                if (slotDto.Id is not null &&
                    existingSlotsDict.TryGetValue(slotDto.Id.Value, out var existingSlot))
                {
                    //  Update
                    if (slotDto.StartTime is not null)
                        existingSlot.StartTime = slotDto.StartTime.Value;

                    if (slotDto.MaxCapacity is not null)
                        existingSlot.MaxCapacity = slotDto.MaxCapacity.Value;

                    if (!string.IsNullOrWhiteSpace(slotDto.AvailableTimeStatus))
                        existingSlot.AvailableTimeStatus = EnumsMapping.ToAvailableSheduleStatus(slotDto.AvailableTimeStatus);
                }
                else
                {
                    //  Create
                    var newSlot = new AvailableSlots
                    {
                        StartTime = slotDto.StartTime ?? default,
                        MaxCapacity = slotDto.MaxCapacity ?? 0,
                        AvailableTimeStatus = EnumsMapping.ToAvailableSheduleStatus(slotDto.AvailableTimeStatus),
                        TourScheduleId = schedule.Id
                    };

                    schedule.AvailableSlots.Add(newSlot);
                }
            }
        }

        public void AddAvailableSlots(
            TourSchedule schedule,
            List<UpdateAvailableSlotsRequestDTO>? slotsDto)
        {
            if (slotsDto is null || slotsDto.Count == 0)
                return;

            foreach (var slotDto in slotsDto)
            {
                var newSlot = new AvailableSlots
                {
                    StartTime = slotDto.StartTime ?? default,
                    MaxCapacity = slotDto.MaxCapacity ?? 0,
                    AvailableTimeStatus = EnumsMapping.ToAvailableSheduleStatus(slotDto.AvailableTimeStatus),
                    TourScheduleId = schedule.Id
                };

                schedule.AvailableSlots.Add(newSlot);
            }
        }
    }
}

using Amigo.Domain.DTO.AvailableSlots;
using Amigo.SharedKernal.DTOs.TourSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.Admin
{
    public class AdminAvailableSlotsService() 
        : IAdminAvailableSlotsService
    {
        public void AddAvailableSlots(TourSchedule schedule, List<UpdateAvailableSlotsRequestDTO>? slotsDto)
        {
            throw new NotImplementedException();
        }

        //public void UpdateAvailableSlots(
        // TourSchedule schedule,
        // List<UpdateAvailableSlotsRequestDTO>? slotsDto)
        //{
        //    if (slotsDto is null || slotsDto.Count == 0)
        //        return;

        //    var existingSlotsDict = schedule.AvailableSlots
        //        .Where(s => s.Id != Guid.Empty)
        //        .ToDictionary(s => s.Id, s => s);

        //    foreach (var slotDto in slotsDto)
        //    {
        //        if (slotDto.Id is not null &&
        //            existingSlotsDict.TryGetValue(slotDto.Id.Value, out var existingSlot))
        //        {
        //            //  Update
        //            if (slotDto.StartTime is not null)
        //                existingSlot.StartTime = slotDto.StartTime.Value;

        //            if (slotDto.EndTime is not null)
        //                existingSlot.EndTime = slotDto.EndTime.Value;

        //            if (slotDto.MaxCapacity is not null)
        //                existingSlot.MaxCapacity = slotDto.MaxCapacity.Value;

        //            if (!string.IsNullOrWhiteSpace(slotDto.AvailableTimeStatus))
        //                existingSlot.AvailableTimeStatus = EnumsMapping.ToAvailableSheduleStatus(slotDto.AvailableTimeStatus);
        //        }
        //        else
        //        {
        //            //  Create
        //            var newSlot = new AvailableSlots
        //            {
        //                Id = slotDto.Id ?? Guid.NewGuid(),
        //                StartTime = slotDto.StartTime ?? default,
        //                EndTime = slotDto.EndTime,
        //                MaxCapacity = slotDto.MaxCapacity ?? 0,
        //                AvailableTimeStatus = EnumsMapping.ToAvailableSheduleStatus(slotDto.AvailableTimeStatus),
        //                TourScheduleId = schedule.Id
        //            };

        //            schedule.AvailableSlots.Add(newSlot);
        //        }
        //    }
        //}

        //public void AddAvailableSlots(
        //    TourSchedule schedule,
        //    List<UpdateAvailableSlotsRequestDTO>? slotsDto)
        //{
        //    if (slotsDto is null || slotsDto.Count == 0)
        //        return;

        //    foreach (var slotDto in slotsDto)
        //    {
        //        var newSlot = new AvailableSlots
        //        {
        //            Id = slotDto.Id ?? Guid.NewGuid(),
        //            StartTime = slotDto.StartTime ?? default,
        //            EndTime = slotDto.EndTime,
        //            MaxCapacity = slotDto.MaxCapacity ?? 0,
        //            AvailableTimeStatus = EnumsMapping.ToAvailableSheduleStatus(slotDto.AvailableTimeStatus),
        //            TourScheduleId = schedule.Id
        //        };

        //        schedule.AvailableSlots.Add(newSlot);
        //    }
        //}

        public void UpdateAvailableSlots(TourSchedule schedule, List<UpdateAvailableSlotsRequestDTO>? slotsDto)
        {
            throw new NotImplementedException();
        }
    }
}

using Amigo.Domain.DTO.AvailableSlots;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public class AvailableSlotsMapping : IAvailableSlotsMapping
    {
        public AvailableSlots AvailableSlotsDTOToEntity(CreateAvailableSlotsRequestDTO requestDTO, TourSchedule tourSchedule)
        {
            var avialableSlots = new AvailableSlots()
            {
                Id = Guid.NewGuid(),
                StartTime = requestDTO.StartTime,
                MaxCapacity = requestDTO.MaxCapacity,
                TourSchedule = tourSchedule,
                TourScheduleId = tourSchedule.Id,


            };
            if (requestDTO.AvailableTimeStatus is not null)
            {
                avialableSlots.AvailableTimeStatus = EnumsMapping.ToAvailableSheduleStatus(requestDTO.AvailableTimeStatus);
            }
            return avialableSlots;
        }
    }
}

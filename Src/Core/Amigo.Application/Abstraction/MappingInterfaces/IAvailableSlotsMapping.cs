using Amigo.Domain.DTO.AvailableSlots;
using Amigo.Domain.DTO.TourSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.MappingInterfaces
{
    public interface IAvailableSlotsMapping
    {
        AvailableSlots AvailableSlotsDTOToEntity(CreateAvailableSlotsRequestDTO requestDTO, TourSchedule tourSchedule);

    }
}

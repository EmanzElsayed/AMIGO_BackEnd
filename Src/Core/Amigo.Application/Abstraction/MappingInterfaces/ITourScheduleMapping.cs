using Amigo.Domain.DTO.TourSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.MappingInterfaces
{
    public interface ITourScheduleMapping
    {
        TourSchedule TourScheduleDTOToEntity(CreateTourScheduleRequestDTO requestDTO, Tour tour);
    }
}

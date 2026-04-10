using Amigo.Domain.DTO.TourSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.MappingInterfaces
{
    public interface ITourScheduleMapping
    {
        List<TourSchedule> TourSchedulesDTOToEntity(List<CreateTourScheduleRequestDTO> requestDTO, Tour tour);
        
    }
}

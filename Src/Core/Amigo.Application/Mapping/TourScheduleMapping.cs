using Amigo.Domain.DTO.TourSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public class TourScheduleMapping : ITourScheduleMapping
    {
        public TourSchedule TourScheduleDTOToEntity(CreateTourScheduleRequestDTO requestDTO, Tour tour)
        {
           
            var shedule = new TourSchedule()
            {
                Id = Guid.NewGuid(),
                StartDate = requestDTO.StartDate,
                EndDate = requestDTO.EndDate,
                Tour = tour,
                TourId =  tour.Id,
               
            };
            if (requestDTO.AvailableDateStatus is not null)
            {
                shedule.AvailableDateStatus = EnumsMapping.ToAvailableSheduleStatus(requestDTO.AvailableDateStatus);
            }
            return shedule;
        }
    }
}

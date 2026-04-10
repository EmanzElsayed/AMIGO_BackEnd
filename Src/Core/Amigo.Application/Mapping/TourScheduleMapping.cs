using Amigo.Domain.DTO.TourSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public class TourScheduleMapping : ITourScheduleMapping
    {
        public List<TourSchedule> TourSchedulesDTOToEntity(List<CreateTourScheduleRequestDTO> requestDTO, Tour tour)
        {

            return requestDTO.Select(requestDTO => new TourSchedule
            {

                Id = Guid.NewGuid(),
                StartDate = requestDTO.StartDate,

                Tour = tour,
                TourId = tour.Id,
                AvailableDateStatus = EnumsMapping.ToEnum<AvailableDateTimeStatus>
                (requestDTO.AvailableDateStatus, false),
                AvailableSlots =  
                requestDTO.availableSlots
                .Select(availableSlotsDTO => new AvailableSlots {

                    Id = Guid.NewGuid(),
                    StartTime = availableSlotsDTO.StartTime,
                    MaxCapacity = availableSlotsDTO.MaxCapacity,
                    AvailableTimeStatus = EnumsMapping.ToEnum<AvailableDateTimeStatus>
                    (availableSlotsDTO.AvailableTimeStatus, false)
                }).ToList()
            }).ToList();
            
           
        }
    }
}

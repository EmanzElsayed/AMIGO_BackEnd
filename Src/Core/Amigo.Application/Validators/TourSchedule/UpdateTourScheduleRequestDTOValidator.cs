using Amigo.Application.Validators.AvailableSlots;
using Amigo.Domain.DTO.TourSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.TourSchedule
{
    public class UpdateTourScheduleRequestDTOValidator :AbstractValidator<UpdateTourScheduleRequestDTO>
    {
        public UpdateTourScheduleRequestDTOValidator()
        {
            RuleFor(x => x.StartDate)
                  .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                  .When(x => x.StartDate is not null)
                  .WithMessage("StartDate must be today or in the future");

            

            RuleFor(x => x.AvailableDateStatus)
              .Must(BusinessRules.BeAValidDateStatus)
              .When(x => !string.IsNullOrEmpty(x.AvailableDateStatus))
              .WithMessage("Invalid Guide AvailableTimeStatus Code (Available, SoldOut, Closed)");


            RuleForEach(x => x.availableSlots)
           .SetValidator(new UpdateAvailableSlotsRequestDTOValidator())
           .When(x => x.availableSlots is not  null);

        }
    }
}

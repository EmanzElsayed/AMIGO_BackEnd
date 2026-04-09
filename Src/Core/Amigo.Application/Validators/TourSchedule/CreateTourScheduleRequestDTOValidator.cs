using Amigo.Domain.DTO.TourSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.TourSchedule
{
    public class CreateTourScheduleRequestDTOValidator:AbstractValidator<CreateTourScheduleRequestDTO>
    {
        public CreateTourScheduleRequestDTOValidator()
        {
            RuleFor(x => x.TourId)
              .NotEmpty()
              .WithMessage("TourId is required");

            // StartDate must be in the future (optional business rule)
            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("StartDate must be today or in the future");

            // EndDate must be >= StartDate
            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("EndDate must be equal or after StartDate");

            RuleFor(x => x.AvailableDateStatus)
              .Must(BusinessRules.BeAValidDateStatus)
              .When(x => !string.IsNullOrEmpty(x.AvailableDateStatus))
              .WithMessage("Invalid Guide AvailableTimeStatus Code (Available, SoldOut, Closed)");
        }
    }
}

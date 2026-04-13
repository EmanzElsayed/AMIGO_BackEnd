using Amigo.Domain.DTO.AvailableSlots;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.AvailableSlots
{
    public class UpdateAvailableSlotsRequestDTOValidator:AbstractValidator<UpdateAvailableSlotsRequestDTO>
    {
        public UpdateAvailableSlotsRequestDTOValidator()
        {
            RuleFor(x => x.StartTime)
              
               .Must(t => t != default)
               .When(t => t.StartTime is not null)
              .WithMessage("StartTime must be a valid time in HH:mm format");

           

            RuleFor(x => x.MaxCapacity)
                .GreaterThan(0)
                .WithMessage("MaxCapacity must be greater than 0")
                .LessThanOrEqualTo(1000)
                .WithMessage("MaxCapacity is too large")
                .When(x => x.MaxCapacity is not null);



            RuleFor(x => x.AvailableTimeStatus)
              .Must(BusinessRules.BeAValidDateStatus)
              .When(x => !string.IsNullOrEmpty(x.AvailableTimeStatus))
              .WithMessage("Invalid Guide AvailableTimeStatus Code (Available, SoldOut, Closed)");
        }
    }
}

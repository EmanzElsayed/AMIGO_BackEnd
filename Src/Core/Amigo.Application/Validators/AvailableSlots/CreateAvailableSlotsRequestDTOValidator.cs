using Amigo.Domain.DTO.AvailableSlots;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.AvailableSlots
{
    public class CreateAvailableSlotsRequestDTOValidator:AbstractValidator<CreateAvailableSlotsRequestDTO>
    {
        public CreateAvailableSlotsRequestDTOValidator()
        {

            RuleFor(x => x.TourScheduleId)
               .NotEmpty()
               .WithMessage("TourScheduleId is required");

            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("StartTime is required")
                 .Must(t => t != default)
                .WithMessage("StartTime must be a valid time in HH:mm format");

            RuleFor(x => x.EndTime)
                .NotEmpty()
                .WithMessage("EndTime is required")
                .Must(t => t != default)
                .WithMessage("EndTime must be a valid time in HH:mm format")
                .GreaterThan(x => x.StartTime)
                .WithMessage("EndTime must be after StartTime");

            RuleFor(x => x.MaxCapacity)
                .GreaterThan(0)
                .WithMessage("MaxCapacity must be greater than 0")
                .LessThanOrEqualTo(1000)
                .WithMessage("MaxCapacity is too large");

          

            RuleFor(x => x.AvailableTimeStatus)
              .Must(BusinessRules.BeAValidDateStatus)
              .When(x => !string.IsNullOrEmpty(x.AvailableTimeStatus))
              .WithMessage("Invalid Guide AvailableTimeStatus Code (Available, SoldOut, Closed)");
        }
    }
}

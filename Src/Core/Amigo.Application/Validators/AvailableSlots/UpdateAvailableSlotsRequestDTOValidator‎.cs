using Amigo.Domain.DTO.AvailableSlots;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.AvailableSlots
{
    public class UpdateAvailableSlotsRequestDTOValidator : AbstractValidator<UpdateAvailableSlotsRequestDTO>
    {
        public UpdateAvailableSlotsRequestDTOValidator()
        {
            RuleFor(x => x.Time)

               .Must(t => t != default)
               .When(t => t.Time is not null)
              .WithMessage("StartTime must be a valid time in HH:mm format");

          



        }
    }
}

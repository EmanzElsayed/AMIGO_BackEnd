using Amigo.Domain.DTO.Price;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Price
{
    public class PriceWithActivityTypeRequestDTOValidator:AbstractValidator<PiceWithActivityTypeRequestDTO>
    {
        public PriceWithActivityTypeRequestDTOValidator()
        {
            RuleFor(x => x.TourId)
                .NotEmpty()
                .WithMessage("Tour Id Required");

            RuleFor(x => x.ActivityType)
                .NotEmpty()
                .WithMessage("Activity Type  Required");
        }
    }
}

using Amigo.Domain.DTO.Price;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Price
{
    public class PriceWithActivityTypeRequestDTOValidator:AbstractValidator<PiceWithActivityTypeRequestQuery>
    {
        public PriceWithActivityTypeRequestDTOValidator()
        {
           
            RuleFor(x => x.ActivityType)
                .NotEmpty()
                .WithMessage("Activity Type  Required");
        }
    }
}

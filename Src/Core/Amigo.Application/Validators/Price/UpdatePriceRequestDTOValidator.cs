using Amigo.Domain.DTO.Price;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Price
{
    public class UpdatePriceRequestDTOValidator:AbstractValidator<UpdatePriceRequestDTO>
    {
        public UpdatePriceRequestDTOValidator()
        {
            RuleFor(x => x.Discount)
             .InclusiveBetween(0, 100)
             .When(x => x.Discount.HasValue)
             .WithMessage("Discount Must Be Between 0 and 100");

            RuleFor(x => x.Cost)
                    .GreaterThan(0)
                    .WithMessage("Cost must be greater than 0")
                    .When(x => x.Cost.HasValue);



            RuleFor(x => x.UserType)
               
               .Must(BusinessRules.IsValidFlagsEnumNullable)
               .WithMessage("User Type Code (VIP, Public)")
               .When(x => x.UserType is not null);
        }
    }
}

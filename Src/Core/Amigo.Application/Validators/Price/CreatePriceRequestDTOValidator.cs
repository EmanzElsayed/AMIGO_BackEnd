using Amigo.Domain.DTO.Price;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Price
{
    public class CreatePriceRequestDTOValidator:AbstractValidator<CreatePriceRequestDTO>
    {
        public CreatePriceRequestDTOValidator()
        {
           

            RuleFor(x => x.Discount)
               .InclusiveBetween(0, 100)
               .When(x => x.Discount.HasValue)
               .WithMessage("Discount Must Be Between 0 and 100");

            RuleFor(x => x.Cost)
                    .GreaterThan(0)
                    .WithMessage("Cost must be greater than 0");


            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage("Type Is Required");

            RuleFor(x => x.UserType)
               .NotEmpty()
               .WithMessage("User Type Is Required")
               .Must(BusinessRules.IsValidFlagsEnum)
               .WithMessage("User Type Code (VIP, Public)");
        }
    }
}

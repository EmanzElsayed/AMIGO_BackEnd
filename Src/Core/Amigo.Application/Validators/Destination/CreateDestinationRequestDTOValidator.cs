using Amigo.Application.Validation.Common.Rules;
using Amigo.Domain.DTO.Destination;
using Amigo.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Destination
{
    public class CreateDestinationRequestDTOValidator:AbstractValidator<CreateDestinationRequestDTO>
    {
        public CreateDestinationRequestDTOValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty()
               .WithMessage("Name Is Required")
               .MaximumLength(300)
               .WithMessage("Length Of Name Must Be Less Than 300");

            RuleFor(x => x.IsActive)
              .NotEmpty()
              .WithMessage("Active Status Is Required");
              

            RuleFor(x => x.CountryCode)
                .NotEmpty()
                .WithMessage("Country Is Required")
                .Must(BusinessRules.BeAValidCountry)
                .WithMessage("Invalid Country Code Must be (Egypt, UAE, Turkey, KSA)");

            RuleFor(x => x.Language)
                .NotEmpty()
                .WithMessage("Language Is Required")
                .Must(BusinessRules.BeAValidLanguage)
                .WithMessage("Invalid Language Code Must be (English, Spanish, SpanishLatinAmerica, French, Italian, PortuguesePortugal, PortugueseBrazil)");

        }
       
    }
}

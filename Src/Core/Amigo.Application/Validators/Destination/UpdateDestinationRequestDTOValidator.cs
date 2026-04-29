using Amigo.Application.Validation.Common.Rules;
using Amigo.Domain.DTO.Destination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Destination
{
    public class UpdateDestinationRequestDTOValidator : AbstractValidator<UpdateDestinationRequestDTO>
    {
        public UpdateDestinationRequestDTOValidator()
        {

            RuleFor(x => x)
                .Must(x =>
                    (string.IsNullOrEmpty(x.Name) && string.IsNullOrEmpty(x.Language)) ||
                    (!string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.Language))
                )
                .WithMessage("Name and Language must be provided together");

            RuleFor(x => x.Name)
               .MaximumLength(300)
               .When(x => x.Name is not null)
               .WithMessage("Length Of Name Must Be Less Than 300");
               

           

            RuleFor(x => x.CountryCode)
                .Must(BusinessRules.BeAValidCountry)
                .When(x => x.CountryCode is not null)
                .WithMessage("Invalid Country Code Must be (EG, UAE, TR, KSA)");

            RuleFor(x => x.Language)
                .Must(BusinessRules.BeAValidLanguage)
                .When(x => x.Language is not null)
                .WithMessage("Invalid Language Code Must be (en, es, SpanishLatinAmerica, fr, it, PortuguesePortugal, PortugueseBrazil)");
           
            RuleFor(x => x.PublicId)
               .NotEmpty()
               .When(x => !string.IsNullOrEmpty(x.ImageUrl))
               .WithMessage("PublicId is required when ImageUrl is provided");
        }
    }
}

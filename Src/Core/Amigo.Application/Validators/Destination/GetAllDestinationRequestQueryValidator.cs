using Amigo.Application.Validation.Common.Rules;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Destination
{
    public class GetAllDestinationRequestQueryValidator:AbstractValidator<GetAllDestinationQuery>
    {
        
        public GetAllDestinationRequestQueryValidator()
        {
            RuleFor(x => x.CountryCode)
                 .Must(BusinessRules.BeAValidCountry)
                 .WithMessage("Invalid Country Code Must be (EG, UAE, TR, KSA)")
                 .When(x => x.CountryCode is not null);

            RuleFor(x => x.Language)
               
                .Must(BusinessRules.BeAValidLanguage)
                .WithMessage("Invalid Language Code Must be (en, es, fr, it, Portuguese (Portugal), Portuguese (Brazil) )")
                .When(x => x.Language is not null);

        }
    }
}

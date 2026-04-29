using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.CountryInfo
{
    public class GetAllCountryInfoQueryValidator:AbstractValidator<GetAllCountryInfoQuery>
    {
        public GetAllCountryInfoQueryValidator()
        {
            RuleFor(c => c.Language)
                .Must(BusinessRules.BeAValidLanguage)
                .WithMessage("Invalid Language Code")
                .When(c => !string.IsNullOrWhiteSpace( c.Language));

            RuleFor(c => c.CountryCode)
               .Must(BusinessRules.BeAValidCountry)
               .WithMessage("Invalid Country Code")
               .When(c => !string.IsNullOrWhiteSpace(c.CountryCode));
        }
    }
}

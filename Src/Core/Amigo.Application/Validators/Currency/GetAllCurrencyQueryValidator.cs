using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Currency
{
    public class GetAllCurrencyQueryValidator:AbstractValidator<GetAllCurrencyQuery>
    {
        public GetAllCurrencyQueryValidator()
        {
            RuleFor(c => c.Language)
               .Must(BusinessRules.BeAValidLanguage)
               .WithMessage("Invalid Language Code")
               .When(c => !string.IsNullOrWhiteSpace(c.Language));

            RuleFor(c => c.CurrencyCode)
               .Must(BusinessRules.BeAValidCurrency)
               .WithMessage("Invalid Country Code")
               .When(c => !string.IsNullOrWhiteSpace(c.CurrencyCode));
        }
    }
}

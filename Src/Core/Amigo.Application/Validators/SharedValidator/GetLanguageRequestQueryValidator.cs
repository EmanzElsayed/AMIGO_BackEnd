using Amigo.Application.Validation.Common.Rules;
using Amigo.Domain.DTO.Destination;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.SharedValidator
{
    public class GetLanguageRequestQueryValidator:AbstractValidator<GetLanuageQuery>
    {
        public GetLanguageRequestQueryValidator()
        {
            RuleFor(x => x.Language)
                 
                .Must(BusinessRules.BeAValidLanguage)
                .When(x => x.Language is not null)
                .WithMessage("Invalid Language Code Must be (en, es, fr, it, Portuguese (Portugal), Portuguese (Brazil) )");

        }
    }
}

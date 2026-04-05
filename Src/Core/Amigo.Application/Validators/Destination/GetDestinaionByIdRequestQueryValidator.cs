using Amigo.Application.Validation.Common.Rules;
using Amigo.Domain.DTO.Destination;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Destination
{
    public class GetDestinaionByIdRequestQueryValidator:AbstractValidator<GetDestinationByIdQuery>
    {
        public GetDestinaionByIdRequestQueryValidator()
        {
            RuleFor(x => x.Language)
                 
                .Must(BusinessRules.BeAValidLanguage)
                .When(x => x.Language is not null)
                .WithMessage("Invalid Language Code Must be (English, Spanish, French, Italian, Portuguese (Portugal), Portuguese (Brazil) )");

        }
    }
}

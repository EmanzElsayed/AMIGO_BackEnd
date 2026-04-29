using Amigo.Application.Validation.Common.Rules;
using Amigo.SharedKernal.QueryParams;

namespace Amigo.Application.Validators.Destination;

public class GetTopDestinationsQueryValidator : AbstractValidator<GetTopDestinationsQuery>
{
    public GetTopDestinationsQueryValidator()
    {
        RuleFor(x => x.Take)
            .InclusiveBetween(1, 50);

        RuleFor(x => x.Language)
            .Must(BusinessRules.BeAValidLanguage)
            .When(x => !string.IsNullOrWhiteSpace(x.Language))
            .WithMessage("Invalid Language Code Must be (en, es, fr, it, Portuguese (Portugal), Portuguese (Brazil) )");

        RuleFor(x => x.Currency)
            .Must(BusinessRules.BeAValidCurrency)
            .When(x => !string.IsNullOrWhiteSpace(x.Currency))
            .WithMessage("Invalid currency code.");
    }
}

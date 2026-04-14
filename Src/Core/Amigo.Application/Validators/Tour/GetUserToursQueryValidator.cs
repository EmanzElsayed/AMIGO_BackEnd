using Amigo.SharedKernal.QueryParams;
using Amigo.Application.Validation.Common.Rules;

namespace Amigo.Application.Validators.Tour;

public class GetUserToursQueryValidator : AbstractValidator<GetUserToursQuery>
{
    public GetUserToursQueryValidator()
    {
        RuleFor(x => x.DestinationId).NotEmpty();

        RuleFor(x => x.Currency)
            .Must(BusinessRules.BeAValidCurrency)
            .When(x => !string.IsNullOrWhiteSpace(x.Currency))
            .WithMessage("Invalid currency code.");

        //Language
        //CountryCode
        //GuideLanguage
        //AvailabilityDate
        //UserType
    }
}

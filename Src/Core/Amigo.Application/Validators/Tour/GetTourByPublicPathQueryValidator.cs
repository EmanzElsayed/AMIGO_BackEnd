using Amigo.SharedKernal.QueryParams;
using FluentValidation;
using Amigo.Application.Validation.Common.Rules;

namespace Amigo.Application.Validators.Tour;

public class GetTourByPublicPathQueryValidator : AbstractValidator<GetTourByPublicPathQuery>
{
    public GetTourByPublicPathQueryValidator()
    {
        RuleFor(x => x.DestinationSlug).NotEmpty();
        RuleFor(x => x.TourSlug).NotEmpty();
        RuleFor(x => x.Currency)
            .Must(BusinessRules.BeAValidCurrency)
            .When(x => !string.IsNullOrWhiteSpace(x.Currency))
            .WithMessage("Invalid currency code.");
    }
}

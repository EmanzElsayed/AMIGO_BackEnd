using Amigo.SharedKernal.DTOs.Tour;
using Amigo.Application.Validation.Common.Rules;
using FluentValidation;

namespace Amigo.Application.Validators.Checkout;

public class CheckoutQuoteRequestDtoValidator : AbstractValidator<CheckoutQuoteRequestDto>
{
    public CheckoutQuoteRequestDtoValidator()
    {
        RuleFor(x => x.TourId).NotEmpty();
        RuleFor(x => x.SlotId).NotEmpty();
        RuleFor(x => x.DateIso).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Currency)
            .Must(BusinessRules.BeAValidCurrency)
            .When(x => !string.IsNullOrWhiteSpace(x.Currency))
            .WithMessage("Invalid currency code.");
        RuleFor(x => x.Tiers).NotEmpty();
        RuleForEach(x => x.Tiers).ChildRules(t =>
        {
            t.RuleFor(x => x.PriceId).NotEmpty();
            t.RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        });
        RuleFor(x => x)
            .Must(x => x.Tiers.Sum(t => t.Quantity) > 0)
            .WithMessage("Select at least one traveler.");
    }
}

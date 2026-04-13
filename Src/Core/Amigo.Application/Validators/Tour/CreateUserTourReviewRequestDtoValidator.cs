using Amigo.SharedKernal.DTOs.Tour;
using FluentValidation;

namespace Amigo.Application.Validators.Tour;

public class CreateUserTourReviewRequestDtoValidator : AbstractValidator<CreateUserTourReviewRequestDto>
{
    public CreateUserTourReviewRequestDtoValidator()
    {
        RuleFor(x => x.TourId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(0.5m, 10m);
        RuleFor(x => x.Comment).NotEmpty().MaximumLength(2000);
        RuleForEach(x => x.ImageUrls).Must(x => !string.IsNullOrWhiteSpace(x)).When(x => x.ImageUrls is not null);
        RuleFor(x => x.ImageUrls).Must(x => x == null || x.Count <= 6)
            .WithMessage("Maximum 6 review images are allowed.");
    }
}

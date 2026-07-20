using Amigo.Domain.DTO.Review;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Review
{
    public class CreateAdminReviewRequestValidator :AbstractValidator<AdminAddReviewsDTO>
    {
        public CreateAdminReviewRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("User Name Requird");

            RuleFor(x => x.UserNationality)
                .NotEmpty()
                .WithMessage("User Nationality  Requird");

            RuleFor(x => x.ToursId)
                .NotEmpty()
                .WithMessage("Select Tours");

            RuleFor(x => x.Rating).InclusiveBetween(0.5m, 10m);
            RuleFor(x => x.Comment).NotEmpty().MaximumLength(2000);
            RuleForEach(x => x.ImageUrls).Must(x => !string.IsNullOrWhiteSpace(x.ImageUrl) && !string.IsNullOrWhiteSpace(x.PublicId))
             .When(x => x.ImageUrls is not null);
            RuleFor(x => x.ImageUrls).Must(x => x == null || x.Count <= 6)
                .WithMessage("Maximum 6 review images are allowed.");
        }
    }
}

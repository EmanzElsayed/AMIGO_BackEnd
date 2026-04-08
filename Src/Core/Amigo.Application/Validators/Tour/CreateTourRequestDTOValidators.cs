using Amigo.Application.Validators.Images;
using Amigo.Domain.DTO.Tour;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Tour
{
    public class CreateTourRequestDTOValidator : AbstractValidator<CreateTourRequestDTO>
    {
        public CreateTourRequestDTOValidator()
        {
            // Title
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title Is Required")
                .MaximumLength(400)
                .WithMessage("Length Of Title Must Be Less Than 400");

            // Description
            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Description Must Be Less Than 2000 Characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            // Language
            RuleFor(x => x.Language)
                .NotEmpty()
                .WithMessage("Language Is Required")
                .Must(BusinessRules.BeAValidLanguage)
                .WithMessage("Invalid Language Code");

            RuleFor(x => x.Currency)
               .NotEmpty()
               .WithMessage("Currency Is Required")
               .Must(BusinessRules.BeAValidCurrency)
               .WithMessage("Invalid Currency Code");

            // Guide Language
            RuleFor(x => x.GuideLanguage)
                .Must(BusinessRules.BeAValidLanguage)
                .When(x => !string.IsNullOrEmpty(x.GuideLanguage))
                .WithMessage("Invalid Guide Language Code");

            // Meeting Point
            RuleFor(x => x.MeetingPoint)
                 .Must(BusinessRules.BeAValidGoogleMapsUrl)
                 .When(x => !string.IsNullOrEmpty(x.MeetingPoint))
                 .WithMessage("Meeting Point must be a valid Google Maps link")
                 .MaximumLength(1000);

            // Duration
            RuleFor(x => x.Duration)
                .GreaterThan(TimeSpan.Zero)
                .WithMessage("Duration Must Be Greater Than Zero");

            // DestinationId
            RuleFor(x => x.DestinationId)
                .NotEmpty()
                .WithMessage("Destination Is Required");

            // Discount
            RuleFor(x => x.Discount)
                .InclusiveBetween(0, 100)
                .When(x => x.Discount.HasValue)
                .WithMessage("Discount Must Be Between 0 and 100");

            RuleForEach(x => x.Images)
                .SetValidator(new ImageUrlsRequestDTOValidator())
                .When(x => x.Images != null);

            RuleFor(x => x)
                .Must(x => x.IsVip || x.IsPublic)
                .WithMessage("At least one of VIP Activity or Public Activity must be true");
        }
    }
}

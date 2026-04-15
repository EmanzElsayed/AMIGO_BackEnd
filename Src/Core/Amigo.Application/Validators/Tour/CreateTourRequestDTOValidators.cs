using Amigo.Application.Validators.Cancellation;
using Amigo.Application.Validators.Images;
using Amigo.Application.Validators.Price;
using Amigo.Application.Validators.TourSchedule;
using Amigo.Domain.DTO.Tour;
using Amigo.Domain.DTO.TourSchedule;
using FluentValidation;
using System;
using System.Linq;

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
               .WithMessage("CurrencyCode Is Required")
               .Must(BusinessRules.BeAValidCurrency)
               .WithMessage("Invalid CurrencyCode Code");

            // Guide Language
            RuleFor(x => x.GuideLanguage)
                .Must(BusinessRules.IsValidFlagsEnumNullable)
                .When(x => x.GuideLanguage is not null)
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


            RuleFor(x => x.UserType)
                .NotEmpty()
                .WithMessage("User Type Is Required")
                .Must(BusinessRules.IsValidFlagsEnum)
                .WithMessage("User Type Code (VIP, Public)");

            RuleForEach(x => x.Images)
                .SetValidator(new ImageUrlsRequestDTOValidator())
                .When(x => x.Images != null);


            RuleForEach(x => x.Schedule)
              .SetValidator(new CreateTourScheduleRequestDTOValidator())
              .When(x => x.Schedule != null);

            RuleFor(x => x)
                .Must(HaveScheduleMatchingDuration)
                .WithMessage("Schedule date range must match the activity duration.");

            RuleForEach(x => x.Prices)
           .SetValidator(new CreatePriceRequestDTOValidator())
           .When(x => x.Prices != null);


            RuleFor(x => x.Cancellation)
          .SetValidator(new CreateCancellationRequestDTOValidator())
          .When(x => x.Cancellation != null);
        }

        private static bool HaveScheduleMatchingDuration(CreateTourRequestDTO request)
        {
            if (request.Schedule is null || request.Schedule.Count == 0)
                return true;

            var totalMinutes = request.Duration.TotalMinutes;
            if (totalMinutes <= 0)
                return false;

            var requiredDaySpan = (int)Math.Floor(request.Duration.TotalDays);
            var uniqueDates = request.Schedule
                .Select(x => x.StartDate)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            if (uniqueDates.Count == 0)
                return false;

            var actualDaySpan = uniqueDates[^1].DayNumber - uniqueDates[0].DayNumber;
            return actualDaySpan == requiredDaySpan;
        }
    }
}

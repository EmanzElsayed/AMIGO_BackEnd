using Amigo.Application.Validators.Cancellation;
using Amigo.Application.Validators.Images;
using Amigo.Application.Validators.Price;
using Amigo.Application.Validators.TourSchedule;
using Amigo.Domain.DTO.Tour;
using System;
using System.Linq;

namespace Amigo.Application.Validators.Tour
{
    public class UpdateTourRequestDTOValidator : AbstractValidator<UpdateTourRequestDTO>
    {
        public UpdateTourRequestDTOValidator()
        {
            // Title
            RuleFor(x => x.Title)

                .MaximumLength(400)
                .WithMessage("Length Of Title Must Be Less Than 400")
                .When(x => !string.IsNullOrEmpty(x.Title));


            // Description
            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Description Must Be Less Than 2000 Characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            // Language
            RuleFor(x => x.Language)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty()
                      .WithMessage("Language is required when updating translation fields")
                      .When(x =>
                          !string.IsNullOrWhiteSpace(x.Title) ||
                          !string.IsNullOrWhiteSpace(x.Description) ||
                          (x.Includes != null && x.Includes.Any()) ||
                          (x.NotIncludes != null && x.NotIncludes.Any()) ||
                          (x.Prices != null && x.Prices.Any())
                      )
                  .Must(BusinessRules.BeAValidLanguage)
                      .WithMessage("Invalid Language Code")
                      .When(x => !string.IsNullOrWhiteSpace(x.Language));


            RuleFor(x => x.Currency)
               .Must(BusinessRules.BeAValidCurrency)
               .WithMessage("Invalid CurrencyCode Code")
               .When(x => x.Currency is not null);



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
                 .MaximumLength(50000);

            // Duration
            RuleFor(x => x.Duration)
                .GreaterThan(TimeSpan.Zero)
                .WithMessage("Duration Must Be Greater Than Zero")
                .When(x => x.Duration is not null);



            RuleFor(x => x.UserType)

                .Must(BusinessRules.IsValidFlagsEnumNullable)
                .WithMessage("User Type Code (VIP, Public)")
                .When(x => x.UserType is not null);


            RuleForEach(x => x.Images)
                .SetValidator(new ImageUrlsRequestDTOValidator())
                .When(x => x.Images != null);


            RuleForEach(x => x.Schedule)
              .SetValidator(new UpdateTourScheduleRequestDTOValidator())
              .When(x => x.Schedule != null);

            RuleFor(x => x)
              .Must(HaveScheduleMatchingDurationWhenProvided)
              .WithMessage("Schedule date range must match the provided activity duration.")
              .When(x => x.Duration is not null && x.Schedule is not null && x.Schedule.Any());

            RuleForEach(x => x.Prices)
           .SetValidator(new UpdatePriceRequestDTOValidator())
           .When(x => x.Prices != null);


            RuleFor(x => x.Cancellation)
            .SetValidator(new UpdateCancellationRequestDTOValidator())
            .When(x => x.Cancellation != null);
        }

        private static bool HaveScheduleMatchingDurationWhenProvided(UpdateTourRequestDTO request)
        {
            if (request.Duration is null || request.Schedule is null || request.Schedule.Count == 0)
                return true;

            var totalMinutes = request.Duration.Value.TotalMinutes;
            if (totalMinutes <= 0)
                return false;

            var requiredDaySpan = (int)Math.Floor(request.Duration.Value.TotalDays);
            var uniqueDates = request.Schedule
                .Where(x => x.StartDate.HasValue)
                .Select(x => x.StartDate!.Value)
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

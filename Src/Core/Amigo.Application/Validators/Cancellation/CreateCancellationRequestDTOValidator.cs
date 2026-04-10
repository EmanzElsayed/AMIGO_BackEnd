using Amigo.Domain.DTO.Cancellation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Cancellation
{
    public class CreateCancellationRequestDTOValidator:AbstractValidator<CreateCancellationRequestDTO>
    {
        public CreateCancellationRequestDTOValidator()
        {
            RuleFor(x => x.CancellationBefore)
             .Must(x => x > TimeSpan.Zero)
             .WithMessage("Cancellation must be greater than zero");

            RuleFor(x => x.RefundPercentage)
               .InclusiveBetween(0, 100)
               .WithMessage("RefundPercentage must be between 0 and 100");


            RuleFor(x => x.Description)
               .MaximumLength(500)
               .When(x => !string.IsNullOrEmpty(x.Description))
               .WithMessage("Description must not exceed 500 characters");

            RuleFor(x => x.CancelationPolicyType)
                .Must(BusinessRules.BeAValidCancellation)
                .When(x => !string.IsNullOrEmpty(x.CancelationPolicyType))
                .WithMessage("Invalid Cancelation Policy Type Code (Free, Paid)");
        }
    }
}

using Amigo.Domain.DTO.Authentication;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Authentication
{
    public class RegisterRequestDTOValidator : AbstractValidator<RegisterRequestDTO>
    {
        private readonly ILocalizationService _localizer;

        public RegisterRequestDTOValidator(ILocalizationService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.FullName)
              .NotEmpty()
              .WithMessage("FullName is required.")
              .MinimumLength(3)
              .WithMessage(_localizer.Get("Validation_MinLength",_localizer.Get("Fields_FullName"),3))
              .MaximumLength(256)
              .WithMessage(_localizer.Get("Validation_MaxLength", _localizer.Get("Fields_FullName"),256));


            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .WithMessage(_localizer.Get("Validation_InvalidEmail")  );
           

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage(_localizer.Get("Validation_MaxLength", _localizer.Get("Fields_Password"),8))
                .Matches(@"[A-Z]")
                .WithMessage(_localizer.Get("Validation_Password_Uppercase"))
                .Matches(@"[a-z]")
                .WithMessage(_localizer.Get("Validation_Password_Lowercase"))
                .Matches(@"\d")
                .WithMessage(_localizer.Get("Validation_Password_Number"))
                .Matches(@"[\W_]")
                .WithMessage(_localizer.Get("Validation_Password_SpecialCharacter"));

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Confirm password is required.")
                .Equal(x => x.Password)
                .WithMessage(_localizer.Get("Validation_Password_NotMatch"));

            RuleFor(x => x.TermsAccepted)
                .Equal(true)
                .WithMessage("You must accept the terms and conditions.");
        }
    }
}

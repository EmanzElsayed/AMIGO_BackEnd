using Amigo.Domain.DTO.Authentication;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Authentication
{
    public class RegisterRequestDTOValidator : AbstractValidator<RegisterRequestDTO>
    {
        public RegisterRequestDTOValidator()
        {

            RuleFor(x => x.FullName)
              .NotEmpty()
              .WithMessage("FullName is required.")
              .MinimumLength(3)
              .WithMessage("FullName must be at least 3 Characters.")
              .MaximumLength(256)
              .WithMessage("FullName must be less than 256 Characters.");


            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .WithMessage("Email must be a valid email address.");
           

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.")
                .Matches(@"[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d")
                .WithMessage("Password must contain at least one number.")
                .Matches(@"[\W_]")
                .WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Confirm password is required.")
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");

            RuleFor(x => x.TermsAccepted)
                .Equal(true)
                .WithMessage("You must accept the terms and conditions.");
        }
    }
}

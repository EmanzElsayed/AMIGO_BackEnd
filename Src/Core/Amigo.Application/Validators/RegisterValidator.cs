using System;
using System.Collections.Generic;
using System.Text;

using Amigo.Domain.DTO.Authentication;

using FluentValidation;

namespace Amigo.Application.Validators;
public class RegisterValidator : AbstractValidator<RegisterRequestDTO>
{
    public RegisterValidator()
    {

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.")
            .Matches(@"^[^@\s]+@nfsc\.sa$")
            .WithMessage("Email must end with @nfsc.sa.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .MinimumLength(2)
            .WithMessage("Full name must be at least 2 characters.")
            .MaximumLength(100)
            .WithMessage("Full name must not exceed 100 characters.");

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
using Amigo.Domain.DTO.Authentication;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators
{
    public class LoginRequestDTOValidator : AbstractValidator<LoginRequestDTO>
    {
        public LoginRequestDTOValidator() {
            RuleFor(x => x.Email)
               .NotEmpty()
               .WithMessage("Email is required.")
               .EmailAddress()
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
        }

    }
}

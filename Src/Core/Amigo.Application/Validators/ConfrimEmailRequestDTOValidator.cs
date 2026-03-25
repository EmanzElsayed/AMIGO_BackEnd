using Amigo.Domain.DTO.Authentication;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators
{
    public class ConfrimEmailRequestDTOValidator : AbstractValidator<ConfirmEmailRequestDTO>
    {
        public ConfrimEmailRequestDTOValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email must be a valid email address.");

            RuleFor(x => x.Token)
               .NotEmpty()
               .WithMessage("Token is required.");
        }
    }
}

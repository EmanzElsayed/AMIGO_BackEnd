using Amigo.Domain.DTO.Authentication;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators
{
    public class ForgetPasswordRequestDTOValidator : AbstractValidator<ForgetPasswordRequestDTO>
    {
        public ForgetPasswordRequestDTOValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email must be a valid email address.");


        }
    }
}

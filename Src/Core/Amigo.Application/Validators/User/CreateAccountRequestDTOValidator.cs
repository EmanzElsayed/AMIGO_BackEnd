using Amigo.Domain.DTO.User;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.User
{
    public class CreateAccountRequestDTOValidator:AbstractValidator<CreateAccountRequestDTO>
    {
        private readonly PhoneNumberUtil _phoneUtil;

        public CreateAccountRequestDTOValidator()
        {
            _phoneUtil = PhoneNumberUtil.GetInstance();

            RuleFor(c => c.FirstName)
                .NotEmpty()
                .WithMessage("First Name Requried");

            RuleFor(c => c.LastName)
               .NotEmpty()
               .WithMessage("LastName Name Requried")
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

            RuleFor(x => x.PhoneNumber)
              .NotEmpty()
              .WithMessage("Phone Number is required.")
              .Must((model, phone) => BeValidPhone(phone, model.CountryIsoCode))
              .WithMessage("Phone Number is invalid.");

        }
        private  bool BeValidPhone(string phone, string region)
        {
            try
            {
                var number = _phoneUtil.Parse(phone, region ?? "EG");
                return _phoneUtil.IsValidNumber(number);
            }
            catch
            {
                return false;
            }
        }
    }
}

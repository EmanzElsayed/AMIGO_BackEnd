using Amigo.Domain.DTO.User;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.User
{
    public class VerifyOTPCheckoutValidator : AbstractValidator<VerifyOTPCheckoutRequestDTO>
    {
        private readonly PhoneNumberUtil _phoneUtil;

        public VerifyOTPCheckoutValidator()
        {
            _phoneUtil = PhoneNumberUtil.GetInstance();

       
            RuleFor(x => x.Email)
               .NotEmpty()
               .WithMessage("Email is required.")
               .EmailAddress()
               .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
               .WithMessage("Email must be a valid email address.");

            RuleFor(x => x.PhoneNumber)
             
             
              .Must((model, phone) => BeValidPhone(phone, model.CountryIsoCode))
              .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber) && !string.IsNullOrWhiteSpace(x.CountryIsoCode))
              .WithMessage("Phone Number is invalid.");

        }
        private bool BeValidPhone(string phone, string region)
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

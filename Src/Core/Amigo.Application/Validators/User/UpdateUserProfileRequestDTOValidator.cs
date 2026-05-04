using Amigo.Domain.DTO.User;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.User
{
    public class UpdateUserProfileRequestDTOValidator:AbstractValidator<UpdateUserProfileRequestDTO>
    {
        private readonly PhoneNumberUtil _phoneUtil;

        public UpdateUserProfileRequestDTOValidator()
        {

            _phoneUtil = PhoneNumberUtil.GetInstance();

            //RuleFor(x => x.Email)
               
            //   .EmailAddress()
            //   .When(x => !string.IsNullOrWhiteSpace(x.Email))
            //   .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            //   .WithMessage("Email must be a valid email address.");

            RuleFor(x => x.CountryIsoCode)
           .NotEmpty()
           .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
           .WithMessage("Country Code is required when PhoneNumber Number is provided");

            RuleFor(x => x.PhoneNumber)              
              .Must((model, phone) => BeValidPhone(phone, model.CountryIsoCode))
              .When(x =>  !string.IsNullOrWhiteSpace(x.PhoneNumber) && !string.IsNullOrWhiteSpace(x.CountryIsoCode))
              .WithMessage("Invalid PhoneNumber Number.");

            RuleFor(x => x.ImagePublicId)
              .NotEmpty()
              .When(x => !string.IsNullOrEmpty(x.ImageUrl))
              .WithMessage("PublicId is required when ImageUrl is provided");

            RuleFor(x => x.Gender)
                .Must(BusinessRules.BeAValidGender)
                 .When(x => !string.IsNullOrWhiteSpace(x.Gender))
                 .WithMessage("Invalid Gender Type.");

            RuleFor(x => x.Language)
           .Must(BusinessRules.BeAValidLanguage)
           .When(x => !string.IsNullOrWhiteSpace(x.Language))
           .WithMessage("Invalid Language Code");


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

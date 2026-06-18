using Amigo.Domain.DTO.Cart;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Cart
{
    public class UpdateCartRequestDTOValidation:AbstractValidator<UpdateCartItemRequestDTO>
    {
        private readonly PhoneNumberUtil _phoneUtil;

        public UpdateCartRequestDTOValidation()
        {
            _phoneUtil = PhoneNumberUtil.GetInstance();
            
            RuleFor(x => x.PhoneNumber)
                
             .Must((model, phone) => BeValidPhone(phone, model.CountryIsoCode))
             .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber) &&  !string.IsNullOrWhiteSpace(x.CountryIsoCode))
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

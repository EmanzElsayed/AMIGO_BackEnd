using Amigo.Application.Helpers;
using Amigo.Domain.DTO.User;
using Amigo.Domain.DTO.PhoneNumber;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class PhoneCodeService : IPhoneCodeService
    {
        private readonly List<CountryPhoneInfo> _countries;

        public PhoneCodeService()
        {
            _countries = CountryPhoneSeed.Countries;
        }

        public Result<IEnumerable<GetPhoneNumberCodeDTO>> GetCountries(string? lang)
        {

            if (string.IsNullOrWhiteSpace(lang)) lang = Language.en.ToString();
            return Result.Ok( _countries.Select(c => new GetPhoneNumberCodeDTO
            {
                IsoCode = c.IsoCode,
                 PhoneCode = c.PhoneCode,
                Name = GetName(c, lang)
            }));
        }

        private string GetName(CountryPhoneInfo country, string lang)
        {
            return country.Names.TryGetValue(lang, out var name)
                ? name
                : country.Names["en"]; // fallback
        }
    }
}

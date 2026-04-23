using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Customer
{
    public class CountryPhoneInfo
    {
        public string IsoCode { get; set; } = null!;
        public string PhoneCode { get; set; } = null!;

        // Key = language, Value = translated name
        public Dictionary<string, string> Names { get; set; } = new();
    }
}

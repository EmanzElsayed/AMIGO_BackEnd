using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.QueryParams
{
    public class GetAllCountryInfoQuery
    {
        public string? CountryCode { get; set; }
        public string? PhoneCode { get; set; }
        public string? Name { get; set; }
        public string? Language { get; set; }
    }
}

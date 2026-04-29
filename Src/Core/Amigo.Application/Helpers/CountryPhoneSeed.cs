using Amigo.Domain.DTO.Customer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Helpers
{
    public static class CountryPhoneSeed
    {
        public static readonly List<CountryPhoneInfo> Countries = new()
    {
        new()
        {
            IsoCode = "EG",
            PhoneCode = "+20",
            Names = new Dictionary<string, string>
            {
                ["en"] = "EG",
                ["es"] = "Egipto",
                ["fr"] = "Égypte",
                ["it"] = "Egitto",
                ["pt-pt"] = "Egito",
                ["pt-pt"] = "Egito"
            }
        },

        new()
        {
            IsoCode = "AE",
            PhoneCode = "+971",
            Names = new Dictionary<string, string>
            {
                ["en"] = "United Arab Emirates",
                ["es"] = "Emiratos Árabes Unidos",
                ["fr"] = "Émirats arabes unis",
                ["it"] = "Emirati Arabi Uniti",
                ["pt-pt"] = "Emirados Árabes Unidos",
                ["pt-pt"] = "Emirados Árabes Unidos"
            }
        },

        new()
        {
            IsoCode = "SA",
            PhoneCode = "+966",
            Names = new Dictionary<string, string>
            {
                ["en"] = "Saudi Arabia",
                ["es"] = "Arabia Saudita",
                ["fr"] = "Arabie saoudite",
                ["it"] = "Arabia Saudita",
                ["pt-pt"] = "Arábia Saudita",
                ["pt-pt"] = "Arábia Saudita"
            }
        },

        new()
        {
            IsoCode = "TR",
            PhoneCode = "+90",
            Names = new Dictionary<string, string>
            {
                ["en"] = "TR",
                ["es"] = "Turquía",
                ["fr"] = "Turquie",
                ["it"] = "Turchia",
                ["pt-pt"] = "Turquia",
                ["pt-pt"] = "Turquia"
            }
        }
    };
    }
}

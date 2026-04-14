using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.AutoTranslation
{
    public static class LanguageMapper
    {
        private static readonly Dictionary<Language, string> Map = new()
        {
            { Language.English, "en" },
            { Language.Espanol, "es" },
            { Language.Francais, "fr" },
            { Language.Italiano, "it" },
            { Language.Portuguese, "pt" },
            { Language.Portuguese_Portugal, "pt" }
        };

        public static string ToCode(Language lang)
        {
            return Map.TryGetValue(lang, out var code)
                ? code
                : throw new Exception($"Unsupported language: {lang}");
        }
    }
}

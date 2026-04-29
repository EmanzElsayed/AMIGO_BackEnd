using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.AutoTranslation
{
    public static class LanguageMapper
    {
        private static readonly Dictionary<Language, string> Map = new()
        {
            { Language.en, "en" },
            { Language.es, "es" },
            { Language.fr, "fr" },
            { Language.it, "it" },
            { Language.br, "pt" },
            { Language.pt, "pt" }
        };

        public static string ToCode(Language lang)
        {
            return Map.TryGetValue(lang, out var code)
                ? code
                : throw new Exception($"Unsupported language: {lang}");
        }
    }
}

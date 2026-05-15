using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.AutoTranslation
{
    public static class LanguageMapper
    {
        private static readonly Dictionary<SupportedLanguage, string> Map = new()
        {
            { SupportedLanguage.en, "en" },
            { SupportedLanguage.es, "es" },
            { SupportedLanguage.fr, "fr" },
            { SupportedLanguage.it, "it" },
            { SupportedLanguage.br, "pt" },
            { SupportedLanguage.pt, "pt" }
        };

        public static string ToCode(SupportedLanguage lang)
        {
            return Map.TryGetValue(lang, out var code)
                ? code
                : throw new Exception($"Unsupported language: {lang}");
        }
    }
}

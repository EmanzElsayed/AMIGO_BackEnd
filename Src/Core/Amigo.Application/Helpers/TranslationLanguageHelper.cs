using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Helpers
{
    public static class TranslationLanguageHelper
    {
        public static List<SupportedLanguage> All =>
        [
            SupportedLanguage.en,
            SupportedLanguage.es,
            SupportedLanguage.fr,
            SupportedLanguage.it,
            SupportedLanguage.pt,
            SupportedLanguage.br
        ];

        public static List<SupportedLanguage>
            GetTargetLanguages(
                SupportedLanguage sourceLanguage)
        {
            return All
                .Where(x => x != sourceLanguage)
                .ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.CountriesInfo
{
    public class GetCountryTranslationWithLanguageSpeficiation : BaseSpecification<CountryInfoTranslation, Guid>
    {
        public GetCountryTranslationWithLanguageSpeficiation(Guid countryId, SupportedLanguage language)
            : base(tr => !tr.IsDeleted && tr.CountryInfoId == countryId && tr.Language == language)
        {
        }
    }
}

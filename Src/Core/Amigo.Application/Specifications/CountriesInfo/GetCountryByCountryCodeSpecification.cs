using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CountriesInfo
{
    public class GetCountryByCountryCodeSpecification : BaseSpecification<CountryInfo, Guid>
    {
        public GetCountryByCountryCodeSpecification(CountryCode countryCode , Language language) 
            : base(c => !c.IsDeleted && c.CountryCode == countryCode  && c.Translations.Any(t => t.Language == language))
        {
            AddInclude(c => c.Translations);
        }
    }
}

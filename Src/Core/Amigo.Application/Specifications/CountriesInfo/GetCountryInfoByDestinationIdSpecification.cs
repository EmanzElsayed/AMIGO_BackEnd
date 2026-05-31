using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CountriesInfo
{
    public class GetCountryInfoByDestinationIdSpecification : BaseSpecification<CountryInfo, Guid>
    {
        public GetCountryInfoByDestinationIdSpecification(Guid destinationId , SupportedLanguage language) 
            : base(c => c.Destinations.Any(d => d.Id == destinationId))
        {
            AddInclude(c => c.Translations);
        }
    }
}

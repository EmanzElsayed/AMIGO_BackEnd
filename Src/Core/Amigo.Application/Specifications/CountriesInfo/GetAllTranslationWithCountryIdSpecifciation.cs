using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CountriesInfo
{
    public class GetAllTranslationWithCountryIdSpecifciation : BaseSpecification<CountryInfoTranslation, Guid>
    {
        public GetAllTranslationWithCountryIdSpecifciation(Guid countryId) 
            : base(tr => !tr.IsDeleted && tr.CountryInfoId == countryId)
        {
        }
    }
}

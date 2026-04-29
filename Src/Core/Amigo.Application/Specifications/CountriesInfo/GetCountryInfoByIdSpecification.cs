using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Amigo.Application.Specifications.CountriesInfo
{
    public class GetCountryInfoByIdSpecification : BaseSpecification<CountryInfo, Guid>
    {
        public GetCountryInfoByIdSpecification(Guid id , Language language) 
            : base(c => !c.IsDeleted && 
                c.Id == id
                && ( c.Translations.Any(t => t.Language == language))

            )
        {
            AddInclude(d => d.Translations
                               .OrderByDescending(t => t.Language == language)
                               .Take(1)
                           );
        }
    }
}

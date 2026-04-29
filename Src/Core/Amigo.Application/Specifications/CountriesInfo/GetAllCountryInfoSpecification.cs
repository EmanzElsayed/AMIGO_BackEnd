using Amigo.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Amigo.Application.Specifications.CountriesInfo
{
    public class GetAllCountryInfoSpecification : BaseSpecification<CountryInfo, Guid>
    {
        public GetAllCountryInfoSpecification(GetAllCountryInfoQuery query,Language language)
            : base(c => !c.IsDeleted 
                 && (string.IsNullOrWhiteSpace(query.CountryCode) || c.CountryCode == EnumsMapping.ToCountryCodeEnum(query.CountryCode))

                && (string.IsNullOrWhiteSpace(query.Name) ||   c.Translations.Any(t => SlugHelper.MatchesName(query.Name,t.Name)))
                && (c.Translations.Any(t => t.Language == language))
            )
        {
            AddInclude(d => d.Translations
                               .OrderByDescending(t => t.Language == language)
                               .Take(1)
                           );
        }
    }
}

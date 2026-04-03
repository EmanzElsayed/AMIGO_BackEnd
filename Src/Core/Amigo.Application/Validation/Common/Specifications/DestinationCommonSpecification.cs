using Amigo.Application.Mapping;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace Amigo.Application.Validation.Common.Specifications
{
    public static class DestinationCommonSpecification
    {
        public static Expression<Func<Destination, bool>> BuildCriteria(
       GetAllDestinationQuery requestQuery, bool isAdmin)
        {
            var language = requestQuery.Language is not null?
                EnumsMapping.ToLanguageEnum(requestQuery.Language) :(Language?) null;
            return d =>

                    (
                    d.Translations.Any( t => 
                        (string.IsNullOrWhiteSpace(requestQuery.Name) ||
                        t.Name.ToLower().Contains(requestQuery.Name.ToLower())
                        )
                         &&
                        (language == null || language == t.Language )
                        )
                    )




                && (string.IsNullOrWhiteSpace(requestQuery.CountryCode)
                    || d.CountryCode == EnumsMapping.ToCountryCodeEnum(requestQuery.CountryCode))

                && (isAdmin || d.IsActive);
                
        }
    }
}

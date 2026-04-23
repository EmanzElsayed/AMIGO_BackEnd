using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Amigo.Application.Specifications.CurrencySpecification
{
    public class GetCurrencyWithQuerySpecification : UserBaseSpecification<Currency, Guid>
    {
        public GetCurrencyWithQuerySpecification(Guid currencyId, GetAllCurrencyQuery query)
            : base(c => c.Id == currencyId && !c.IsDeleted
             && (string.IsNullOrWhiteSpace(query.CurrencyCode) || c.CurrencyCode == EnumsMapping.ToEnum<CurrencyCode>(query.CurrencyCode, false))
                && (string.IsNullOrWhiteSpace(query.Name) || c.Translations.Any(t => t.Name.ToLower().Trim().Contains(query.Name)))
                && (string.IsNullOrWhiteSpace(query.Language) || c.Translations.Any(t => t.Language == EnumsMapping.ToLanguageEnum(query.Language)))
            )
        {
            AddInclude(c => c.Translations);
        }
}   }

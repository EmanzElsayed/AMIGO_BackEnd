using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CurrencySpecification
{
    public class GetAllCurrencySpecification : BaseSpecification<Currency, Guid>
    {
        public GetAllCurrencySpecification(GetAllCurrencyQuery query) 
            : base(c => !c.IsDeleted
                && (string.IsNullOrWhiteSpace(query.CurrencyCode) || c.CurrencyCode == EnumsMapping.ToEnum<CurrencyCode>(query.CurrencyCode , false))
                && (string.IsNullOrWhiteSpace(query.Name) || c.Translations.Any(t => t.Name.ToLower().Trim().Contains(query.Name)))
                && (string.IsNullOrWhiteSpace(query.Language) || c.Translations.Any(t => t.Language == EnumsMapping.ToLanguageEnum(query.Language)))

            )
        {
            AddInclude(c => c.Translations);
        }
    }
}

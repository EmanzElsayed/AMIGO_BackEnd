using Amigo.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CurrencySpecification
{
    public class GetAllCurrencySpecification : BaseSpecification<Currency, Guid>
    {
        public GetAllCurrencySpecification(GetAllCurrencyQuery query , Language language) 
            : base(c => !c.IsDeleted
                && (string.IsNullOrWhiteSpace(query.CurrencyCode) || c.CurrencyCode == EnumsMapping.ToEnum<CurrencyCode>(query.CurrencyCode , false))

                && (string.IsNullOrWhiteSpace(query.Name) || c.Translations.Any(t => SlugHelper.MatchesName(query.Name, t.Name)))
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

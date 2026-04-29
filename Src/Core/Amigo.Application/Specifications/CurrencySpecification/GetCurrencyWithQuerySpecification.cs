using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Amigo.Application.Specifications.CurrencySpecification
{
    public class GetCurrencyWithQuerySpecification : BaseSpecification<Currency, Guid>
    {
        public GetCurrencyWithQuerySpecification(Guid currencyId, Language language)
            : base(c => c.Id == currencyId && !c.IsDeleted
                && (c.Translations.Any(t => t.Language == language))
            )
        {
            AddInclude(d => d.Translations
                                .OrderByDescending(t => t.Language == language)
                                .Take(1)
                            );
        }
}   }

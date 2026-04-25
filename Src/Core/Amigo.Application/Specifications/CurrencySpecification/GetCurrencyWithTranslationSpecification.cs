using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CurrencySpecification
{
    public class GetCurrencyWithTranslationSpecification : BaseSpecification<Currency, Guid>
    {
        public GetCurrencyWithTranslationSpecification(Guid currencyId) 
            : base(c => c.Id == currencyId && !c.IsDeleted)
        {
            AddInclude(c => c.Translations);
        }
    }
}

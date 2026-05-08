using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CurrencyRateSpecification
{
    public class GetCurrencyRateWithCurrencyTypeSpecification : BaseSpecification<CurrencyRate, Guid>
    {
        public GetCurrencyRateWithCurrencyTypeSpecification(CurrencyCode from , CurrencyCode to) 
            : base(c => !c.IsDeleted && c.BaseCurrency == from && c.TargetCurrency == to)
        {
        }
    }
}

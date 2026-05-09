using Amigo.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CurrencyRateSpecification
{
    public class GetAllCurrencyWithTargetSpecification : BaseSpecification<CurrencyRate, Guid>
    {
        public GetAllCurrencyWithTargetSpecification(List<CurrencyCode> targets) 
            : base(c => !c.IsDeleted && c.BaseCurrency == Constants.BaseCurrency && targets.Contains(c.TargetCurrency))
        {
        }
    }
}

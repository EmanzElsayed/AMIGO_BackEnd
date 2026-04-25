using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.PaymentSpecification
{
    public class GetPaymentByPaymentIntentSpecification : BaseSpecification<Payment, Guid>
    {
        public GetPaymentByPaymentIntentSpecification(string intentId) 
            : base(p => p.PaymentProviderReferenceId == intentId && !p.IsDeleted)
        {
        }
    }
}

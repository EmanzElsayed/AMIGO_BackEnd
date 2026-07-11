using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.PaymentSpecification
{
    public class GetPaymentByProviderReferenceIdSpecification : BaseSpecification<Payment, Guid>
    {
        public GetPaymentByProviderReferenceIdSpecification(string providerRef)
            : base(p => p.PaymentProviderReferenceId == providerRef && !p.IsDeleted)
        {
            AddInclude(p => p.Order);
        }
    }
}

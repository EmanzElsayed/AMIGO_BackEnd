using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.PaymentSpecification
{
    public class GetPaymentByProviderRefSpec
        : BaseSpecification<Payment, Guid>
    {
        public GetPaymentByProviderRefSpec(string CaptureId)
            : base(p => p.ProviderCaptureId == CaptureId && !p.IsDeleted)
        {
        }
    }
}

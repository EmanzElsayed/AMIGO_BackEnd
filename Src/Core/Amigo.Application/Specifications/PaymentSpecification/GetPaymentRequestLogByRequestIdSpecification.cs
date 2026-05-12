using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.PaymentSpecification
{
    public class GetPaymentRequestLogByRequestIdSpecification : BaseSpecification<PaymentRequestLog, Guid>
    {
        public GetPaymentRequestLogByRequestIdSpecification(string requestId) 
            : base(p => !p.IsDeleted && p.RequestId == requestId)
        {
        }
    }
}

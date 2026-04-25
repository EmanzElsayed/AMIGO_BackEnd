using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.PaymentSpecification
{
    internal class GetPaymentByOrderIdSpecification : BaseSpecification<Payment, Guid>
    {
        public GetPaymentByOrderIdSpecification(Guid orderId)
            : base(p => !p.IsDeleted && p.OrderId == orderId)
        {
            {

            }  
        }
    }
}

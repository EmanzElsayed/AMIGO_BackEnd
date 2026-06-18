using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.PaymentSpecification
{
    public class GetPendingPaymentWithOrderIdsSpecifciation : BaseSpecification<Payment, Guid>
    {
        public GetPendingPaymentWithOrderIdsSpecifciation(HashSet<Guid>orderIds) 
            : base(p => !p.IsDeleted && orderIds.Contains(p.OrderId) && p.Status == PaymentStatus.Pending)
        {
        }
    }
}

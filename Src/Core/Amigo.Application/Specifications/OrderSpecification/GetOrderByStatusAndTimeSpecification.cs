using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.OrderSpecification
{
    public class GetOrderByStatusAndTimeSpecification : BaseSpecification<Order, Guid>
    {
        public GetOrderByStatusAndTimeSpecification(DateTime limit ) 
            : base(o => o.OrderDate < limit && o.Status == OrderStatus.PendingPayment)
        {
        }
    }
}

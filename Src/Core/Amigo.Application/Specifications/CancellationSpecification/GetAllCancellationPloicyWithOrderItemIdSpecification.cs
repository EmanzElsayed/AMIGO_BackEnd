using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CancellationSpecification
{
    public class GetAllCancellationPloicyWithOrderItemIdSpecification : BaseSpecification<OrderItemCancellationPolicy, Guid>
    {
        public GetAllCancellationPloicyWithOrderItemIdSpecification(Guid orderItemId , TimeSpan remaining) 
            : base(c => !c.IsDeleted && c.OrderItemId == orderItemId && remaining >= c.CancellationBefore)
        {
        }
    }
}

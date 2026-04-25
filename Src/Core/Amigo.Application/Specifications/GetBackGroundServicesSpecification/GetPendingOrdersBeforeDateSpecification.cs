using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.GetBackGroundServicesSpecification
{
    public class GetPendingOrdersBeforeDateSpecification
    : BaseSpecification<Order, Guid>
    {
        public GetPendingOrdersBeforeDateSpecification(DateTime now)
            : base(o =>
                o.Status == OrderStatus.PendingPayment &&
                o.ExpiresAt < now && !o.IsDeleted)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.GetBackGroundServicesSpecification
{
    public class GetOldPendingPaymentsSpecification
     : BaseSpecification<Payment, Guid>
    {
        public GetOldPendingPaymentsSpecification(DateTime limit)
            : base(p =>
                p.Status == PaymentStatus.Pending &&
                p.CreatedDate < limit && !p.IsDeleted)
        {
        }
    }
}

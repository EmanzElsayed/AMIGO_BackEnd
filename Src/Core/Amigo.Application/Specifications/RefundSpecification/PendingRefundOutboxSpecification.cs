using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.RefundSpecification
{
    public class PendingRefundOutboxSpecification
     : BaseSpecification<OutboxMessage, Guid>
    {
        public PendingRefundOutboxSpecification(DateTime now) : base(x =>
                x.Type == "RefundRequested"
                &&
                x.Status == OutboxStatus.Pending
                &&
                (
                    x.NextRetryAt == null
                    ||
                    x.NextRetryAt <= now
                ))
        {
        }
    }
}

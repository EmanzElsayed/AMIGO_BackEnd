using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.WebhookEventLogSpecification
{
    internal class GetPendingOutboxSpecification : BaseSpecification<OutboxMessage, Guid>
    {
        public GetPendingOutboxSpecification(DateTime now, int take)
            : base(x =>
                !x.IsDeleted &&
                x.Status == OutboxStatus.Pending &&
                (x.NextRetryAt == null || x.NextRetryAt <= now))
        {
            ApplyPagination(take, 1);
            AddOrderBY(x => x.CreatedDate);
        }
    }
}

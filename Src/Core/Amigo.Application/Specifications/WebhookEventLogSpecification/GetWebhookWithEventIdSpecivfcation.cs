using Microsoft.Extensions.Logging;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.WebhookEventLogSpecification
{
    public class GetWebhookWithEventIdSpecivfcation : BaseSpecification<WebhookEventLog, Guid>
    {
        public GetWebhookWithEventIdSpecivfcation(string eventId) 
            : base(e => e.ProviderEventId == eventId && !e.IsDeleted)
        {
        }
    }
}

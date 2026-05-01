using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities;
[Table($"{nameof(WebhookEventLog)}", Schema = SchemaConstants.booking_schema)]


public class WebhookEventLog:BaseEntity<Guid>
{
    public PaymentProvider Provider { get; set; } 
    public string ProviderEventId { get; set; } 
    public string Payload { get; set; } 

    public bool Processed { get; set; }
}

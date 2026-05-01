using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities;
[Table($"{nameof(OutboxMessage)}", Schema = SchemaConstants.booking_schema)]


public class OutboxMessage:BaseEntity<Guid>
{
    public string Type { get; set; } 
    public string Payload { get; set; } 

    public OutboxStatus Status { get; set; }
    public int RetryCount { get; set; }

    public DateTime? ProcessedAtUtc { get; set; }
    public DateTime? NextRetryAt { get; set; }

    public string? LastError { get; set; }
}

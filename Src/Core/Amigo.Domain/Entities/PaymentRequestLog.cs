using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities;
[Table($"{nameof(PaymentRequestLog)}", Schema = SchemaConstants.payment_schema)]

public class PaymentRequestLog :BaseEntity<Guid>
{

    public string RequestId { get; set; }

    public Guid OrderId { get; set; }

    public string ProviderPaymentId { get; set; }

    public string ResponseJson { get; set; }

    public DateTime CreatedAt { get; set; }
}

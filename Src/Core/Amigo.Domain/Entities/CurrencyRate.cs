using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities;
[Table($"{nameof(CurrencyRate)}", Schema = SchemaConstants.shared_schema)]

public class CurrencyRate : BaseEntity<Guid>
{
    public CurrencyCode BaseCurrency { get; set; } = CurrencyCode.USD;

    public CurrencyCode TargetCurrency { get; set; }

    public decimal Rate { get; set; }

    public string? Provider { get; set; } 

    public DateTime FetchedAt { get; set; }

    public DateTime ExpiresAt { get; set; }
}

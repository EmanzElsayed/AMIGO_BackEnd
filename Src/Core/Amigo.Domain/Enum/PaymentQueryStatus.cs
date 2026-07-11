using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Enum
{
    public enum PaymentQueryStatus
    {
        Pending = 0,
        Succeeded = 1,
        Failed = 2,
        Cancelled = 3
    }
}

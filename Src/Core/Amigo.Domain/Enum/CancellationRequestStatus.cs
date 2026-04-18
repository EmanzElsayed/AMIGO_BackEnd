using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Enum
{
    public enum CancellationRequestStatus
    {
        Pending = 1,
        Approved = 2,

        Rejected = 3,
        Refunded = 4
    }
}

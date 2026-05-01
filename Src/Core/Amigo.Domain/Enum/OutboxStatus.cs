using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Enum
{
    public enum OutboxStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Enum
{
    [Flags]
    public enum UserType
    {
        None = 0,
        VIP = 1,
        Public = 2

    }
}

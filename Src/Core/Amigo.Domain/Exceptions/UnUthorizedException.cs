using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Exceptions
{
    public sealed class UnUthorizedException(string msg = "Invalid Email or Password") : Exception(msg)
    {
    }
}

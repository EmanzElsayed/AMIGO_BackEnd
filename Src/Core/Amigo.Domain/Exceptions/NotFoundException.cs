using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Exceptions
{
    public class NotFoundException(string msg) : Exception(msg)
    {
    }
}

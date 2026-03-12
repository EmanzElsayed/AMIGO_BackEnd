using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Exceptions.AlreadyExistExceptions
{
    public class AlreadyExistException(string msg) : Exception(msg)
    {
    }
}

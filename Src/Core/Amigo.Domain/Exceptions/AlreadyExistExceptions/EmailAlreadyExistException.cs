using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Exceptions.AlreadyExistExceptions
{
    public sealed class EmailAlreadyExistException(string email)
        : AlreadyExistException($"This Email {email} Already Exist")
    {
    }
}

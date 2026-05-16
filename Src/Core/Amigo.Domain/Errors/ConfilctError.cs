using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Errors
{
    public class ConfilctError : BaseDomainError
    {
        public ConfilctError(string msg)
        : base(
              ErrorCode.ConflictError.ToString()
              ,msg
              )
        {
        }
    }
}

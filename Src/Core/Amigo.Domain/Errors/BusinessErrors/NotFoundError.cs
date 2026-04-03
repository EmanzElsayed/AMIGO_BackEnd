using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Errors.BusinessErrors
{
    public class NotFoundError : BaseDomainError
    {
        public NotFoundError(string msg)
            :base(msg,ErrorCode.NotFoundError)
        {
            
        }
    }
}

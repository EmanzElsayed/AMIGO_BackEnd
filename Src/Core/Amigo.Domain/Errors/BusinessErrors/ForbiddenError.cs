using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Errors.BusinessErrors
{
    public class ForbiddenError :BaseDomainError
    {
        public ForbiddenError(string msg):base(msg,ErrorCode.Forbidden)
        {
            
        }
    }
}

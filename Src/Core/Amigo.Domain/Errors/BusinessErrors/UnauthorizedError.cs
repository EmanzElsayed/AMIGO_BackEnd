using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Errors.BusinessErrors
{
    public class UnauthorizedError :BaseDomainError
    {
        public UnauthorizedError(string msg = "Invalid Email Or Password")
            :base(msg,ErrorCode.Unauthorized)
        {
            
        }
    }
}

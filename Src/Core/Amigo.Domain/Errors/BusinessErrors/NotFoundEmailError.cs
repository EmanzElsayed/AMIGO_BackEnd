using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Errors.BusinessErrors
{
    public class NotFoundEmailError
        : BaseDomainError
    {
        public NotFoundEmailError(string email)
            :base (ErrorCode.NotFoundError.ToString(),email)
        {
            
        }
    }
}

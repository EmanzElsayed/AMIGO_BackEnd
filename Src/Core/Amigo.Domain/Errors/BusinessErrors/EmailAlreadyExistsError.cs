using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Amigo.Domain.Errors.BusinessErrors
{
    public class EmailAlreadyExistsError:BaseDomainError
    {
        public EmailAlreadyExistsError(string email)
        : base($"Email '{email}' already exists.",
              ErrorCode.EmailAlreadyExist
              )
        { 
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Errors.BusinessErrors
{
    public class EmailNotConfirmedError : BaseDomainError
    {
        public EmailNotConfirmedError(string email)
            : base("Auth_EmailNotConfirmed", email)
        {
            
        }
    }
}

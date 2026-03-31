using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Errors
{
    public class ExceptionError : BaseDomainError
    {
        public string? Details { get; }

        public ExceptionError(string message = "Internal Server Error", string? details = null)
            : base(message, ErrorCode.InternalServerError)
        {
            Details = details;
        }
    }
}

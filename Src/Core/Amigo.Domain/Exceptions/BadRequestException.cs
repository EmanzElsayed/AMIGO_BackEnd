using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Exceptions
{
    public class BadRequestException(Dictionary<string, string> errors)
        : Exception("Validation Faild")
    {
        public Dictionary<string, string> Errors { get; set; } = errors;
    }
}

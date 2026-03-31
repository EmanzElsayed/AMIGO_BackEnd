using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Results
{
    public class ApiValidationError
    {
        public string Property { get; set; }
        public IEnumerable<string> Messages { get; set; }
    }
}

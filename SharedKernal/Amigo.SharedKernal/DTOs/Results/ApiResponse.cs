using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Results
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }
        public string? ErrorCode { get; set; }
        public IEnumerable<ApiValidationError>? Errors { get; set; }

        public string? TraceId { get; set; } // مهم جدًا للـ debugging
    }
}

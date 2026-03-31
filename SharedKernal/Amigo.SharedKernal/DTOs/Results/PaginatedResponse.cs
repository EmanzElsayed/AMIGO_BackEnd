using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Results
{
    public class PaginatedResponse<T>
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

       
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public string? TraceId { get; set; }
    }
}

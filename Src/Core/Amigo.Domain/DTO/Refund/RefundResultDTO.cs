using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Refund
{
    public class RefundResultDTO
    {
        public bool Success { get; set; }

        public string? RefundId { get; set; }

        public string? RawResponse { get; set; }

        public string? FailureReason { get; set; }
    }
}

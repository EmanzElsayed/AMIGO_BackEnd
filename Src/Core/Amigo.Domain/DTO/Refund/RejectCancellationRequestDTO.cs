using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Refund
{
    public record RejectCancellationRequestDTO
    (
        string? RejectionReason = null
        
        );
}

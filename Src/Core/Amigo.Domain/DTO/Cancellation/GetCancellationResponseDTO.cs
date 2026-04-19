using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cancellation
{
    public record GetCancellationResponseDTO
    (
       Guid Id,
       string CancelationPolicyType,
       TimeSpan CancellationBefore,
       decimal RefundPercentage,
       string Description
   );
}

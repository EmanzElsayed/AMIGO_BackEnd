using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cancellation
{
    public record UpdateCancellationRequestDTO
   (
       
       string CancelationPolicyType,
       TimeSpan CancellationBefore,
       decimal RefundPercentage
       
   );
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Payment
{
    public class CapturePaymentResponseDTO
    {
        public string PaymentProviderReferenceId { get; set; } = default!;
        public string CaptureId { get; set; }

        public bool Success { get; set; }
        public string? Status { get; set; }
    }
}

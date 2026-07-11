using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Payment
{
    public sealed record QueryPaymentResponseDTO(
    string Status,
    string ProviderReferenceId,
    string? RawResponse
    );
}

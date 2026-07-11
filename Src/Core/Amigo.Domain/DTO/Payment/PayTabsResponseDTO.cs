using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Amigo.Domain.DTO.Payment
{
    public class PayTabsResponseDTO
    {
        [JsonPropertyName("tran_ref")]
        public string TranRef { get; set; }

        [JsonPropertyName("redirect_url")]
        public string? RedirectUrl { get; set; }

        [JsonPropertyName("payment_result")]
        public PaymentResult? PaymentResult { get; set; }
    }
}

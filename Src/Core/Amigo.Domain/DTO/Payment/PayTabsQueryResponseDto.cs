using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Amigo.Domain.DTO.Payment
{
    public sealed class PayTabsQueryResponseDto
    {
        [JsonPropertyName("tran_ref")]
        public string TranRef { get; set; }

        [JsonPropertyName("payment_result")]
        public PayTabsPaymentResultDto PaymentResult { get; set; }
    }
    public sealed class PayTabsPaymentResultDto
    {
        [JsonPropertyName("response_status")]
        public string ResponseStatus { get; set; }

        [JsonPropertyName("response_code")]
        public string ResponseCode { get; set; }
    }
}

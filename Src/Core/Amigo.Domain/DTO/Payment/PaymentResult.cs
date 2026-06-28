using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Amigo.Domain.DTO.Payment
{
    public class PaymentResult
    {
        [JsonPropertyName("response_status")]
        public string ResponseStatus { get; set; }

        [JsonPropertyName("response_code")]
        public string ResponseCode { get; set; }
    }
}

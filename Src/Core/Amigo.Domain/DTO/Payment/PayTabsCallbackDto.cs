using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Amigo.Domain.DTO.Payment
{
    public sealed class PayTabsCallbackDto
    {
        [JsonPropertyName("tran_ref")]
        public string TranRef { get; set; }

        [JsonPropertyName("respStatus")]
        public string RespStatus { get; set; }

        [JsonPropertyName("respCode")]
        public string RespCode { get; set; }
    }
}

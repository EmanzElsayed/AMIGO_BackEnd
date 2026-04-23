using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.PhoneNumber
{
    public class GetPhoneNumberCodeDTO
    {
        public string Name { get; set; } = null!;
        public string IsoCode { get; set; } = null!;
        public string PhoneCode { get; set; } = null!;
    }
}

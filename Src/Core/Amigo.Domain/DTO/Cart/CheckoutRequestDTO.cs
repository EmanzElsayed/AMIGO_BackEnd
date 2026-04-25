using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cart
{
    public class CheckoutRequestDTO
    {
        public PaymentProvider PaymentProvider { get; set; }
    }
}

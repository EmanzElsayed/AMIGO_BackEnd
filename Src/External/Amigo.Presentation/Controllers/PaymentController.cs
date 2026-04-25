using Amigo.Application.Abstraction.Services;
using Amigo.Domain.Abstraction;
using Amigo.Domain.DTO.Payment;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1/payment")]

    public class PaymentController(IPaymentService _service ):BaseController
    {
        [HttpPost("create")]
        public async Task<IResultBase> Create(CreatePaymentRequestDTO dto)
        {
            return await _service.CreatePaymentAsync(dto);
        }

        [HttpPost("{paymentId:guid}/capture")]
        public async Task<IResultBase> Capture(Guid paymentId)
        {
            return await _service.CapturePaymentAsync(paymentId);
        }

       
    }
}

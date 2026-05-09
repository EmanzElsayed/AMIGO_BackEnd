using Amigo.Application.Abstraction.Services;
using Amigo.Domain.Abstraction;
using Amigo.Domain.DTO.Payment;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Microsoft.AspNetCore.RateLimiting;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1/payment")]

    public class PaymentController(IPaymentService _service ):BaseController
    {
        [EnableRateLimiting("booking")]
        [HttpPost("create")]
        public async Task<IResultBase> Create(CreatePaymentRequestDTO dto)
        {
            var requestId = Request.Headers["PayPal-Request-Id"]
             .FirstOrDefault();
            if (string.IsNullOrWhiteSpace(requestId))
            {
                return Result.Fail(
                    "Missing PayPal-Request-Id");
            }
            return await _service.CreatePaymentAsync(dto,requestId);
        }
        [EnableRateLimiting("booking")]
        [HttpPost("{paymentId:guid}/capture")]
        public async Task<IResultBase> Capture(Guid paymentId)
        {
            return await _service.CapturePaymentAsync(paymentId);
        }

       
    }
}

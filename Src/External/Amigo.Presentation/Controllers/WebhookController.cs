using Amigo.Application.Abstraction.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1/webhook")]
    public class WebhookController(IConfiguration _config , IPaymentService _paymentService) : BaseController
    {
        [HttpPost("stripe")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _config["Stripe:WebhookSecret"]
            );

            switch (stripeEvent.Type)
            {
                case "payment_intent.payment_failed":
                    await _paymentService.HandlePaymentFailed(stripeEvent);
                    break;

                case "payment_intent.succeeded":
                    await _paymentService.HandlePaymentSucceeded(stripeEvent);
                    break;
            }

            return Ok();
        }
    }
}

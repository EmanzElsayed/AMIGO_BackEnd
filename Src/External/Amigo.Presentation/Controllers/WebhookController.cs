using Amigo.Application.Abstraction.Services;
using Amigo.Domain.Enum;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Amigo.Presentation.Controllers
{
    [Route("api/webhook")]
    public class WebhookController(IConfiguration _config , IPaymentOrchestrator _orchestrator,
            IPaymentProviderResolver _resolver, ILogger<WebhookController> _logger) : BaseController
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
                    await _orchestrator.HandleFailureAsync(PaymentProvider.Stripe, json);
                    break;

                case "payment_intent.succeeded":
                    await _orchestrator.HandleSuccessAsync(PaymentProvider.Stripe, json);
                    break;
            }

            return Ok();
        }
        [HttpPost("paypal")]
        public async Task<IActionResult> PaypalWebhook()
        {
            try
            {

                //var json = await new StreamReader(Request.Body).ReadToEndAsync();
                //_logger.LogInformation("PayPal webhook received: {json}", json);

                //var provider = _resolver.Resolve(PaymentProvider.Paypal);

                //var isValid = await provider.VerifyWebhookAsync(Request, json);


                //if (!isValid)
                //    return Unauthorized();
                var json = "emanmohamed";
                //var eventType = JsonDocument.Parse(json)
                //    .RootElement.GetProperty("event_type").GetString();
                //_logger.LogInformation("PayPal event type: {type}", eventType);
                var eventType = "PAYMENT.CAPTURE.COMPLETED";
                switch (eventType)
                {
                    case "PAYMENT.CAPTURE.COMPLETED":
                        await _orchestrator.HandleSuccessAsync(PaymentProvider.Paypal, json);
                        break;

                    case "PAYMENT.CAPTURE.DENIED":
                        await _orchestrator.HandleFailureAsync(PaymentProvider.Paypal, json);
                        break;
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PayPal webhook failed");
                return StatusCode(500);
            }
          
        }
    }
}

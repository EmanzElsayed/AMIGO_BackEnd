using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Payment;
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
    public class WebhookController(IConfiguration _config ,IServiceManager _serviceManager
            , ILogger<WebhookController> _logger) : BaseController
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
                    await _serviceManager.PaymentOrchestrator.HandleFailureAsync(PaymentProvider.Stripe, json);
                    break;

                case "payment_intent.succeeded":
                    await _serviceManager.PaymentOrchestrator.HandleSuccessAsync(PaymentProvider.Stripe, json);
                    break;
            }

            return Ok();
        }
        [HttpPost("paypal")]
        public async Task<IActionResult> PaypalWebhook()
        {
            try
            {

               
                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                _logger.LogInformation("PayPal webhook received: {json}", json);

                var provider = _serviceManager.PaymentProviderResolver.Resolve(PaymentProvider.Paypal);
                _logger.LogInformation("STEP 1");

                var isValid = await provider.VerifyWebhookAsync(Request, json);

                _logger.LogInformation("STEP 2");
                if (!isValid)
                    return Unauthorized();
                //var root = "3333";
                var root = JsonDocument.Parse(json)
                    .RootElement;
                var eventType = JsonDocument.Parse(json)
                    .RootElement.GetProperty("event_type").GetString();
                _logger.LogInformation("PayPal event type: {type}", eventType);

                //var json = "eman";
                //var eventType = "PAYMENT.CAPTURE.COMPLETED";
                switch (eventType)
                {
                    case "PAYMENT.CAPTURE.COMPLETED":
                        _logger.LogInformation("STEP 3");
                        await _serviceManager.PaymentOrchestrator.HandleSuccessAsync(PaymentProvider.Paypal, json);
                        break;

                    case "PAYMENT.CAPTURE.DENIED":
                        await _serviceManager.PaymentOrchestrator.HandleFailureAsync(PaymentProvider.Paypal, json);
                        break;

                    case "PAYMENT.CAPTURE.REFUNDED":
                        _logger.LogInformation("refunded send");

                        await _serviceManager.PaymentOrchestrator.HandleRefundCompleted(root);
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

        [HttpPost("paytabs")]
        public async Task<IActionResult> PayTabsCallback()
        {
            var json =
                await new StreamReader(Request.Body)
                    .ReadToEndAsync();
            var callback =
                    JsonSerializer.Deserialize<
                        PayTabsCallbackDto>(json);
            if (callback is null)
                return BadRequest();

            if (string.IsNullOrWhiteSpace(callback.TranRef))
                return BadRequest();

            if (callback.RespStatus == "A")
            {
                await _serviceManager
                    .PaymentOrchestrator
                    .HandleSuccessAsync(
                        PaymentProvider.PayTabs,
                        json);
            }
            else
            {
                await _serviceManager
                    .PaymentOrchestrator
                    .HandleFailureAsync(
                        PaymentProvider.PayTabs,
                        json);
            }

            return Ok();
        }
    }
}

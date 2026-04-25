using PayPalCheckoutSdk.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class PaypalClientFactory
    {
        public PayPalHttpClient CreateClient(IConfiguration config)
        {
            var environment = new SandboxEnvironment(
                config["Paypal:ClientId"],
                config["Paypal:Secret"]
            );

            return new PayPalHttpClient(environment);
        }
    }
}

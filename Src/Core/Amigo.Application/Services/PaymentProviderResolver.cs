using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class PaymentProviderResolver : IPaymentProviderResolver
    {
        private readonly IEnumerable<IPaymentProvider> _providers;

        public PaymentProviderResolver(IEnumerable<IPaymentProvider> providers)
        {
            _providers = providers;
        }

        public IPaymentProvider Resolve(PaymentProvider provider)
        {
            return _providers.First(x => x.Provider == provider);
        }
    }
}

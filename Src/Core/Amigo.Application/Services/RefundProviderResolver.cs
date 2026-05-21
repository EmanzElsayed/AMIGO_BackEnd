using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class RefundProviderResolver
    : IRefundProviderResolver
    {
        private readonly IEnumerable<IRefundProvider>
            _providers;

        public RefundProviderResolver(
            IEnumerable<IRefundProvider> providers)
        {
            _providers = providers;
        }

        public IRefundProvider Resolve(
            PaymentProvider provider)
        {
            return _providers.First(x =>
                x.Provider == provider);
        }
    }
}

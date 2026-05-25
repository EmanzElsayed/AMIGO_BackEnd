using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IRefundProviderResolver
    {
        IRefundProvider Resolve(
            PaymentProvider provider);
    }
}

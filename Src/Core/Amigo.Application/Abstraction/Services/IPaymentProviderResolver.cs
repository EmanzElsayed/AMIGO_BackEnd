using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IPaymentProviderResolver
    {
        IPaymentProvider Resolve(PaymentProvider provider);
    }
}

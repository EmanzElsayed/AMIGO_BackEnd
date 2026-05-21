using Amigo.Domain.DTO.Refund;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IRefundProvider
    {
        PaymentProvider Provider { get; }

        Task<RefundResultDTO> RefundAsync(
            string captureId,
            decimal amount,
            CurrencyCode currency,
            string idempotencyKey);
    }
}

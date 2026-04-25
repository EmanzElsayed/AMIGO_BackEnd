using Amigo.Application.Specifications.PaymentSpecification;
using Amigo.Domain.DTO.Payment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class PaymentService(
    IUnitOfWork _unitOfWork,
    IPaymentProviderResolver _resolver) :IPaymentService

    {
        public async Task<Result<CreatePaymentResponseDTO>> CreatePaymentAsync(CreatePaymentRequestDTO dto)
        {
            var strategy = _unitOfWork.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _unitOfWork.BeginTransactionAsync();

                var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
                var paymentRepo = _unitOfWork.GetRepository<Payment, Guid>();

                var order = await orderRepo.GetByIdAsync(dto.OrderId);

                if (order is null)
                    return Result.Fail("Order not found");

                if (order.Status != OrderStatus.PendingPayment)
                    return Result.Fail("Order is not available for payment");

                // 2. Find existing payment

                var payment = await paymentRepo.GetByIdAsync(
                    new GetPaymentByOrderIdSpecification(dto.OrderId));

                if (payment is null)
                    return Result.Fail("Payment not found");

                if (payment.Status != PaymentStatus.Pending)
                    return Result.Fail("Payment already processed");

                // 3. Resolve provider
                var provider = _resolver.Resolve(dto.Provider);

                // 4. Call provider
                var providerResult = await provider.CreatePaymentAsync(order);

                // 5. Save provider reference
                payment.PaymentProviderReferenceId = providerResult.PaymentIntentId;
                payment.Provider = dto.Provider;

                await _unitOfWork.SaveChangesAsync();

                await tx.CommitAsync();

               


                return Result.Ok(new CreatePaymentResponseDTO(
                    PaymentIntentId: providerResult.PaymentIntentId,
                   RequiresRedirect: providerResult.RequiresRedirect,
                   ClientSecret: providerResult.ClientSecret,
                   RedirectUrl: providerResult.RedirectUrl,
                    paymentId: payment.Id
                ));
            });
        }

        public async Task<Result<CapturePaymentResponseDTO>> CapturePaymentAsync(Guid paymentId)
        {
            var paymentRepo = _unitOfWork.GetRepository<Payment, Guid>();

            var payment = await paymentRepo.GetByIdAsync(paymentId);

            if (payment is null)
                return Result.Fail("Payment not found");

            var provider = _resolver.Resolve(payment.Provider.Value);
            

            var result = await provider.CapturePaymentAsync(payment.PaymentProviderReferenceId);
            
            return Result.Ok(new CapturePaymentResponseDTO
            {
                PaymentProviderReferenceId = payment.PaymentProviderReferenceId,
                Success = true,
                Status = result
            });
        }

    }
}

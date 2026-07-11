using Amigo.Application.Abstraction.Services;
using Amigo.Application.Specifications.PaymentSpecification;
using Amigo.Domain.DTO.Payment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class PaymentService(
    IUnitOfWork _unitOfWork,
    IPaymentProviderResolver _resolver,
        IPaymentOrchestrator _paymentOrchestrator) :IPaymentService

    {
        public async Task<Result<CreatePaymentResponseDTO>> CreatePaymentAsync(CreatePaymentRequestDTO dto,string requestId)
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
                var providerResult = await provider.CreatePaymentAsync(order,requestId,dto.PaymentToken);

                // 5. Save provider reference
                payment.PaymentProviderReferenceId = providerResult.PaymentIntentId;
                payment.Provider = dto.Provider;
                if (string.IsNullOrWhiteSpace(dto.PaymentMethod) && dto.Provider == PaymentProvider.Paypal)
                    payment.PaymentMethod = PaymentMethod.PayPal_balance;

                if (dto.Provider == PaymentProvider.PayTabs)
                { 
                     payment.PaymentMethod = string.IsNullOrWhiteSpace(dto.PaymentMethod) ? PaymentMethod.MasterCard : EnumsMapping.ToEnum<PaymentMethod>(dto.PaymentMethod, false);
                    
                }
                
                
                await _unitOfWork.SaveChangesAsync();

                await tx.CommitAsync();

               


                return Result.Ok(new CreatePaymentResponseDTO(
                    PaymentIntentId: providerResult.PaymentIntentId,
                   RequiresRedirect: providerResult.RequiresRedirect,
                   ClientSecret: providerResult.ClientSecret,
                   RedirectUrl: providerResult.RedirectUrl,
                   CurrencyCode: payment.Currency.ToString(),
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

            if (payment.Status == PaymentStatus.Captured)
                return Result.Fail("Payment already captured");

            if (payment.Status == PaymentStatus.Failed)
                return Result.Fail("Cannot capture failed payment");

            if (payment.Status != PaymentStatus.Pending 
                )
                return Result.Fail("Invalid payment state");

            var provider = _resolver.Resolve(payment.Provider.Value);
            

            var result = await provider.CapturePaymentAsync(payment.PaymentProviderReferenceId);
            
            
            if (result.Success)
            {
                payment.Status = PaymentStatus.Captured;
                payment.ProviderCaptureId =
                    result.CaptureId;

                payment.SetModifiedDate(DateTime.UtcNow) ;
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
            }
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new CapturePaymentResponseDTO
            {
                PaymentProviderReferenceId = payment.PaymentProviderReferenceId,
                Success = true,
                Status = result.Status
            });
        }

        public async Task<Result<PayTabsStatusResponseDTO>> PayTabsStatus(string tranRef)
        {
            if (string.IsNullOrWhiteSpace(tranRef))
            {
               return Result.Fail("TranRef Required");
            }
            var payment = await _unitOfWork.GetRepository<Payment, Guid>().GetByIdAsync(new GetPaymentByPaymentIntentSpecification(tranRef));

            if (payment is null)
            { 
               return Result.Fail("Payment Not Found");

            }

            if (payment.Status == PaymentStatus.Pending)
            { 
                await RecoverPayment(tranRef);
            }
            var result = new PayTabsStatusResponseDTO(Status: payment.Status.ToString());
            return Result.Ok(result);
        }

        public async Task RecoverPayment(
            string tranRef)
        {
            var provider =
                _resolver.Resolve(
                    PaymentProvider.PayTabs);

            var result =
                await provider.QueryTransactionAsync(
                    tranRef);

            switch (result.Status)
            {
                case "Succeeded":
                    await _paymentOrchestrator
                        .HandleRecoveredSuccessAsync(
                            PaymentProvider.PayTabs,
                            result);
                    break;

                case "Failed":
                case "Cancelled":
                    await _paymentOrchestrator
                        .HandleRecoveredFailureAsync(
                            PaymentProvider.PayTabs,
                            result);
                    break;

                case "Pending":
                    break;
            }
        }
    }
}

using Amigo.Domain.DTO.Payment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IPaymentService
    {
        Task<Result<CreatePaymentResponseDTO>> CreatePaymentAsync(CreatePaymentRequestDTO dto);

        Task<Result<CapturePaymentResponseDTO>> CapturePaymentAsync(Guid paymentId);

        //Task<Result<bool>> CancelPaymentAsync(Guid paymentId);
    }
}

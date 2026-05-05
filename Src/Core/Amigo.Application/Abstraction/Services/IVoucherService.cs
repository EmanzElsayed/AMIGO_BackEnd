using Amigo.Domain.DTO.Voucher;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IVoucherService
    {
        Task SendVoucherEmail(Booking booking);
        Task<Result<GetValidateVoucherDTO>> ValidateVoucher(string token);
        Task SendReminderEmail(Booking booking);


    }
}

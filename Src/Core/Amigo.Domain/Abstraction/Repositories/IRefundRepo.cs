using Amigo.Domain.DTO.Refund;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction.Repositories
{
    public interface IRefundRepo
    {
        Task<RefundDetailsForUserDTO?> GetRefundDetails(Guid bookingId);
    }
}

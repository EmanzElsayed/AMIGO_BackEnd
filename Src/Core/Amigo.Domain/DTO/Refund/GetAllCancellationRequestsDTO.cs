using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Refund
{
    public record GetAllCancellationRequestsDTO
    (
         Guid Id,
         Guid BookingId,
         string BookingNumber,
         Guid? TourId,
         string TourTitle,
         DateTime TourDate,
         string UserName,
         string UserEmail,
         
         decimal PaidAmount,
         decimal RefundAmount,
         DateTime RequestedAt,
         string Status,
         string Reason,
         string CancellationPolicyType,
         decimal RefundPercentage

    );
}

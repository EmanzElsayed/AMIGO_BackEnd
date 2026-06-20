using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Refund
{
    public record RefundDetailsForUserDTO
    (
         string TourTitle,
        string DestinationName,
        DateOnly TourDate,
        TimeOnly StartTime,
         string RefundStaus,
         string BookingStatus,
         decimal PaidAmount,
         decimal RefundAmount,
         string CancellationPolicyType,
         decimal RefundPercentage,
         string? PaymentMethod,
          string PaymentStatus,
          string PaidCurrency,
          DateTime? RefundAt,
          string? Provider,
          Guid? TourId,
         string? TourImage = null


    );
}

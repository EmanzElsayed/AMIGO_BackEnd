using Amigo.Domain.Abstraction.Repositories;
using Amigo.Domain.DTO.Refund;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.Repositories
{
    public class RefundRepo(AmigoDbContext _dbContext) : IRefundRepo
    {
        public async Task<RefundDetailsForUserDTO?> GetRefundDetails(Guid bookingId)
        {
            var refund =  await _dbContext.Refunds.AsNoTracking()
                             .Where(r => !r.IsDeleted && r.BookingId == bookingId && r.Status == Domain.Enum.RefundStatus.Completed)
                             .Select(r => new RefundDetailsForUserDTO
                             (
                                 r.Booking.OrderItem.TourTitle,
                                 r.Booking.OrderItem.DestinationName,
                                 r.Booking.OrderItem.TourDate,
                                 r.Booking.OrderItem.StartTime,
                                 r.Status.ToString(),
                                 r.Booking.Status.ToString(),
                                 r.Booking.Payment.TotalAmount,
                                 r.Amount,
                                 r.CancellationRequest.cancelationPolicyType.ToString(),
                                 r.CancellationRequest.RefundPercentage,
                                 r.Payment.PaymentMethod.ToString(),
                                 r.Payment.Status.ToString(),
                                 r.Payment.Currency.ToString(),
                                 r.RefundedAt,
                                 r.Payment.Provider.ToString(),
                                 r.Booking.OrderItem.TourId,
                                 null


                             )).FirstOrDefaultAsync();

            if (refund is  null) return null;

            var tourImage =  refund.TourId is null ? null 
                                : await _dbContext.TourImages.AsNoTracking()
                                .Where(i => i.TourId == refund.TourId)
                                .Select(i => i.ImageUrl)
                                .FirstOrDefaultAsync();
            refund = refund with
            {
                TourImage = tourImage
            };
            
            return refund;


        }
    }
}

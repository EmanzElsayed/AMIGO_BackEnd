using Amigo.Domain.DTO.Booking;
using Amigo.Domain.DTO.Refund;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IBookingService
    {
       
        Task<Result<IEnumerable<UserBookingDTO>>> GetUserBookingsAsync(string userId, string? paymentStatus = null);

        Task FinalizeBooking(Guid paymentId);

        Task<Result> BookingCancellation(string bookingId , CancellationRequestDTO requestDTO,string userId);
        Task<Result> RemoveBookingCancellation(string bookingId,string userId);

        Task<Result<RefundDetailsForUserDTO>> GetRefundDetails(string Id, string UserId);
    }
}

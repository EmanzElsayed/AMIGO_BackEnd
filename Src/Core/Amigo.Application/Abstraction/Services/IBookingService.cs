using Amigo.Domain.DTO.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IBookingService
    {
       
        Task<Result<IEnumerable<UserBookingDTO>>> GetUserBookingsAsync(string userId, string? paymentStatus = null);

        Task FinalizeBooking(Guid paymentId);

    }
}

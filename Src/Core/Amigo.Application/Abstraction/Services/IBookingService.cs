using Amigo.Domain.DTO.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IBookingService
    {
        Task<Result<TravelersSavedResponseDTO>> AddTravelersAsync(
        Guid bookingId,
        string userId,
        AddTravelersRequestDTO dto);
        Task<Result<IEnumerable<UserBookingDTO>>> GetUserBookingsAsync(string userId, string? paymentStatus = null);
    }
}

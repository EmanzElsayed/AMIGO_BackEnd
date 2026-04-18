using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Booking;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1/booking")]

    public class BookingController(IBookingService _bookingService):BaseController
    {
        [HttpPost("{bookingId}/travelers")]
        public async Task<IResultBase> AddTravelers(
            Guid bookingId,
            [FromBody] AddTravelersRequestDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return await _bookingService.AddTravelersAsync(
                bookingId,
                userId,
                dto);
        }
    }
}

using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Refund;
using Amigo.Domain.Errors.BusinessErrors;
using Microsoft.AspNetCore.Authorization;
using org.apache.zookeeper.data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Amigo.Presentation.Controllers.User
{
    [Route("api/v1/user/booking")]

    public class BookingController(IBookingService _bookingService):BaseController
    {
        [HttpPost("{bookingId}/cancel-request")]
        [Authorize]
        public async Task<IResultBase> BookingCancellation(string bookingId ,[FromBody] CancellationRequestDTO requestDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Result.Fail(new UnauthorizedError("Unauthorized"));
            }
            return await _bookingService.BookingCancellation(bookingId,requestDTO,userId);
        }
    }
}

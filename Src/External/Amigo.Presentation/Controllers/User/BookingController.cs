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

    public class BookingController(IServiceManager _serviceManager):BaseController
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
            return await _serviceManager.BookingService.BookingCancellation(bookingId,requestDTO,userId);
        }
        [HttpPost("{bookingId}/remove-cancel-request")]
        [Authorize]
        public async Task<IResultBase> RemoveBookingCancellation(string bookingId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Result.Fail(new UnauthorizedError("Unauthorized"));
            }
            return await _serviceManager.BookingService.RemoveBookingCancellation(bookingId, userId);
        }
        [HttpGet("{bookingId}/refund-details")]
        [Authorize]
        public async Task<IResultBase> RefundDetails(string bookingId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Result.Fail(new UnauthorizedError("Unauthorized"));
            }
            return await _serviceManager.BookingService.GetRefundDetails(bookingId, userId);
        }
    }
}

using Amigo.Application.Abstraction.Services;
using Amigo.Application.Helpers;
using Amigo.Domain.DTO.User;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.Identity;
using Amigo.Domain.Enum;
using Amigo.Domain.Errors.BusinessErrors;
using Amigo.Persistence;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using Amigo.Domain.DTO.Favorite;

namespace Amigo.Presentation.Controllers.User;

[Authorize]
[Route("api/v1/user")]
public class UserAccountController(IUserService _userService, IFavoriteService _favoriteService) 
    : BaseController
{

    [HttpGet("profile")]
    public async Task<IResultBase> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Unauthorized"));

        return await _userService.GetUserProfile(userId);        


    }
    [HttpPatch("profile")]
    public async Task<IResultBase> UpdateProfile([FromBody] UpdateUserProfileRequestDTO requestDTO)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Unauthorized"));

        return await _userService.UpdateUserProfile(requestDTO,userId);


    }
    [HttpGet("favorites")]
    public async Task<IResultBase> GetFavorites()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));

        return await _favoriteService.GetFavoritesAsync(userId);
    }

    [HttpPost("favorites")]
    public async Task<IResultBase> AddFavorite([FromBody] FavoriteRequestDTO body)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));
            
        if (body is null || body.TourId == Guid.Empty)
            return Result.Fail("tourId is required");

        return await _favoriteService.AddFavoriteAsync(userId, body.TourId);
    }

    [HttpDelete("favorites/{tourId:guid}")]
    public async Task<IResultBase> RemoveFavorite([FromRoute] Guid tourId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));

        return await _favoriteService.RemoveFavoriteAsync(userId, tourId);
    }

    //[HttpGet("bookings")]
    //public async Task<IResultBase> GetBookings([FromQuery] string? paymentStatus = null)
    //{
    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //    if (string.IsNullOrWhiteSpace(userId))
    //        return Result.Fail(new UnauthorizedError("Not authenticated"));

    //    return await _bookingService.GetUserBookingsAsync(userId, paymentStatus);
    //}

    
}


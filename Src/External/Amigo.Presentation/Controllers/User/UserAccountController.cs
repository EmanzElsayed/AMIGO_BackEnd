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

namespace Amigo.Presentation.Controllers.User;

[Authorize]
[Route("api/v1/user")]
public class UserAccountController(IUserService _userService) 
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
    //[HttpGet("favorites")]
    //public async Task<IResultBase> GetFavorites()
    //{
    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //    if (string.IsNullOrWhiteSpace(userId))
    //        return Result.Fail(new UnauthorizedError("Not authenticated"));

    //    var rows = await db.Favorites.AsNoTracking()
    //        .Where(f => f.UserId == userId && !f.IsDeleted)
    //        .Join(db.Tours.AsNoTracking().Where(t => !t.IsDeleted),
    //            f => f.TourId,
    //            t => t.Id,
    //            (f, t) => new { f, t })
    //        .Select(x => new
    //        {
    //            tourId = x.t.Id,
    //            title = x.t.Translations
    //                .Where(tr => !tr.IsDeleted)
    //                .OrderBy(tr => tr.Language == Language.en ? 0 : 1)
    //                .Select(tr => tr.Title)
    //                .FirstOrDefault() ?? "Tour",
    //            destinationId = x.t.DestinationId,
    //            coverImageUrl = db.TourImages
    //                .Where(img => img.TourId == x.t.Id && !img.IsDeleted)
    //                .Select(img => img.ImageUrl)
    //                .FirstOrDefault()
    //        })
    //        .ToListAsync();

    //    return Result.Ok(rows);
    //}

    //[HttpPost("favorites")]
    //public async Task<IResultBase> AddFavorite([FromBody] FavoriteRequest body)
    //{
    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //    if (string.IsNullOrWhiteSpace(userId))
    //        return Result.Fail(new UnauthorizedError("Not authenticated"));
    //    if (body is null || body.TourId == Guid.Empty)
    //        return Result.Fail("tourId is required");

    //    var exists = await db.Favorites.AnyAsync(f =>
    //        f.UserId == userId && f.TourId == body.TourId && !f.IsDeleted);
    //    if (exists)
    //        return Result.Ok(new { tourId = body.TourId, isFavorite = true });

    //    var tourExists = await db.Tours.AnyAsync(t => t.Id == body.TourId && !t.IsDeleted);
    //    if (!tourExists)
    //        return Result.Fail(new NotFoundError("Tour not found"));

    //    var entity = new Favorites
    //    {
    //        UserId = userId,
    //        TourId = body.TourId,
    //        User = null!,
    //        Tour = null!,
    //    };

    //    await db.Favorites.AddAsync(entity);
    //    await db.SaveChangesAsync();
    //    return Result.Ok(new { tourId = body.TourId, isFavorite = true });
    //}

    //[HttpDelete("favorites/{tourId:guid}")]
    //public async Task<IResultBase> RemoveFavorite([FromRoute] Guid tourId)
    //{
    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //    if (string.IsNullOrWhiteSpace(userId))
    //        return Result.Fail(new UnauthorizedError("Not authenticated"));

    //    var row = await db.Favorites
    //        .FirstOrDefaultAsync(f => f.UserId == userId && f.TourId == tourId && !f.IsDeleted);
    //    if (row is null)
    //        return Result.Ok(new { tourId, isFavorite = false });

    //    row.SetIsDeleted(true);
    //    db.Favorites.Update(row);
    //    await db.SaveChangesAsync();

    //    return Result.Ok(new { tourId, isFavorite = false });
    //}

    //[HttpGet("bookings")]
    //public async Task<IResultBase> GetBookings([FromQuery] string? paymentStatus = null)
    //{
    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //    if (string.IsNullOrWhiteSpace(userId))
    //        return Result.Fail(new UnauthorizedError("Not authenticated"));

    //    return await _bookingService.GetUserBookingsAsync(userId, paymentStatus);
    //}

    
}

public class FavoriteRequest
{
    public Guid TourId { get; set; }
}


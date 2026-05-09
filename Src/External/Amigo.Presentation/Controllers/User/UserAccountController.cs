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
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Amigo.Presentation.Controllers.User;

[Authorize]
[Route("api/v1/user")]
public class UserAccountController(IUserService _userService, AmigoDbContext _db) 
    : BaseController
{
    [EnableRateLimiting("token")]
    [HttpGet("profile")]
    public async Task<IResultBase> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Unauthorized"));

        return await _userService.GetUserProfile(userId);        


    }
    [EnableRateLimiting("token")]
    [HttpPatch("profile")]
    public async Task<IResultBase> UpdateProfile([FromBody] UpdateUserProfileRequestDTO requestDTO)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Unauthorized"));

        return await _userService.UpdateUserProfile(requestDTO,userId);


    }
    [EnableRateLimiting("token")]

    [HttpGet("favorites")]
    public async Task<IResultBase> GetFavorites()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));

        var rows = await _db.Favorites.AsNoTracking()
            .Where(f => f.UserId == userId && !f.IsDeleted)
            .Join(_db.Tours.AsNoTracking().Where(t => !t.IsDeleted),
                f => f.TourId,
                t => t.Id,
                (f, t) => new { f, t })
            .Join(_db.Destinations.AsNoTracking().Where(d => !d.IsDeleted),
                combined => combined.t.DestinationId,
                d => d.Id,
                (combined, d) => new { combined.f, combined.t, d })
            .Select(x => new
            {
                tourId = x.t.Id,
                title = x.t.Translations
                    .Where(tr => !tr.IsDeleted)
                    .OrderBy(tr => tr.Language == Language.en ? 0 : 1)
                    .Select(tr => tr.Title)
                    .FirstOrDefault() ?? "Tour",
                destinationId = x.t.DestinationId,
                destinationName = x.d.Translations
                    .Where(dt => !dt.IsDeleted)
                    .OrderBy(dt => dt.Language == Language.en ? 0 : 1)
                    .Select(dt => dt.Name)
                    .FirstOrDefault() ?? "Destination",
                coverImageUrl = _db.TourImages
                    .Where(img => img.TourId == x.t.Id && !img.IsDeleted)
                    .Select(img => img.ImageUrl)
                    .FirstOrDefault()
            })
            .ToListAsync();

        var result = rows.Select(r => new
        {
            r.tourId,
            r.title,
            r.destinationId,
            destinationSlug = SlugHelper.ToUrlSlug(r.destinationName),
            tourSlug = SlugHelper.ToUrlSlug(r.title),
            r.coverImageUrl
        });

        return Result.Ok(result);
    }
    [EnableRateLimiting("token")]

    [HttpPost("favorites")]
    public async Task<IResultBase> AddFavorite([FromBody] FavoriteRequest body)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));
        if (body is null || body.TourId == Guid.Empty)
            return Result.Fail("tourId is required");

        var exists = await _db.Favorites.AnyAsync(f =>
            f.UserId == userId && f.TourId == body.TourId && !f.IsDeleted);
        if (exists)
            return Result.Ok(new { tourId = body.TourId, isFavorite = true });

        var tourExists = await _db.Tours.AnyAsync(t => t.Id == body.TourId && !t.IsDeleted);
        if (!tourExists)
            return Result.Fail(new NotFoundError("Tour not found"));

        var entity = new Favorites
        {
            UserId = userId,
            TourId = body.TourId,
            User = null!,
            Tour = null!,
        };

        await _db.Favorites.AddAsync(entity);
        await _db.SaveChangesAsync();
        return Result.Ok(new { tourId = body.TourId, isFavorite = true });
    }
    [EnableRateLimiting("token")]

    [HttpDelete("favorites/{tourId:guid}")]
    public async Task<IResultBase> RemoveFavorite([FromRoute] Guid tourId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));

        var row = await _db.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.TourId == tourId && !f.IsDeleted);
        if (row is null)
            return Result.Ok(new { tourId, isFavorite = false });

        row.SetIsDeleted(true);
        _db.Favorites.Update(row);
        await _db.SaveChangesAsync();

        return Result.Ok(new { tourId, isFavorite = false });
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

public class FavoriteRequest
{
    public Guid TourId { get; set; }
}


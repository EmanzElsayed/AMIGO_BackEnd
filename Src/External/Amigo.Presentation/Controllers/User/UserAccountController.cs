using Amigo.Application.Helpers;
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
public class UserAccountController(AmigoDbContext db, UserManager<ApplicationUser> userManager) : BaseController
{
    [HttpGet("profile")]
    public async Task<IResultBase> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));

        var user = await db.Users.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                id = u.Id,
                fullName = u.FullName ?? u.UserName ?? "",
                email = u.Email ?? ""
            })
            .FirstOrDefaultAsync();

        if (user is null)
            return Result.Fail(new NotFoundError("User not found"));

        var appUser = await userManager.FindByIdAsync(user.id);
        var isVip = appUser is not null && await userManager.IsInRoleAsync(appUser, "VIP");
        var userType = isVip ? "VIP" : "Public";

        return Result.Ok(new
        {
            fullName = user.fullName,
            email = user.email,
            userType
        });
    }

    [HttpGet("favorites")]
    public async Task<IResultBase> GetFavorites()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));

        var rows = await db.Favorites.AsNoTracking()
            .Where(f => f.UserId == userId && !f.IsDeleted)
            .Join(db.Tours.AsNoTracking().Where(t => !t.IsDeleted),
                f => f.TourId,
                t => t.Id,
                (f, t) => new { f, t })
            .Select(x => new
            {
                tourId = x.t.Id,
                title = x.t.Translations
                    .Where(tr => !tr.IsDeleted)
                    .OrderBy(tr => tr.Language == Language.English ? 0 : 1)
                    .Select(tr => tr.Title)
                    .FirstOrDefault() ?? "Tour",
                destinationId = x.t.DestinationId,
                coverImageUrl = db.TourImages
                    .Where(img => img.TourId == x.t.Id && !img.IsDeleted)
                    .Select(img => img.ImageUrl)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Result.Ok(rows);
    }

    [HttpPost("favorites")]
    public async Task<IResultBase> AddFavorite([FromBody] FavoriteRequest body)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));
        if (body is null || body.TourId == Guid.Empty)
            return Result.Fail("tourId is required");

        var exists = await db.Favorites.AnyAsync(f =>
            f.UserId == userId && f.TourId == body.TourId && !f.IsDeleted);
        if (exists)
            return Result.Ok(new { tourId = body.TourId, isFavorite = true });

        var tourExists = await db.Tours.AnyAsync(t => t.Id == body.TourId && !t.IsDeleted);
        if (!tourExists)
            return Result.Fail(new NotFoundError("Tour not found"));

        var entity = new Favorites
        {
            UserId = userId,
            TourId = body.TourId,
            User = null!,
            Tour = null!,
        };

        await db.Favorites.AddAsync(entity);
        await db.SaveChangesAsync();
        return Result.Ok(new { tourId = body.TourId, isFavorite = true });
    }

    [HttpDelete("favorites/{tourId:guid}")]
    public async Task<IResultBase> RemoveFavorite([FromRoute] Guid tourId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));

        var row = await db.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.TourId == tourId && !f.IsDeleted);
        if (row is null)
            return Result.Ok(new { tourId, isFavorite = false });

        row.SetIsDeleted(true);
        db.Favorites.Update(row);
        await db.SaveChangesAsync();

        return Result.Ok(new { tourId, isFavorite = false });
    }

    [HttpGet("bookings")]
    public async Task<IResultBase> GetBookings([FromQuery] string? paymentStatus = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));

        var rows = await (
            from b in db.Bookings.AsNoTracking()
            join o in db.Orders.AsNoTracking() on b.OrderId equals o.Id
            join pay in db.Payments.AsNoTracking() on o.Id equals pay.OrderId
            join slot in db.AvailableSlots.AsNoTracking() on b.AvailableSlotsId equals slot.Id
            join schedule in db.TourSchedules.AsNoTracking() on slot.TourScheduleId equals schedule.Id
            join tour in db.Tours.AsNoTracking() on schedule.TourId equals tour.Id
            where !b.IsDeleted && !o.IsDeleted && !pay.IsDeleted && !slot.IsDeleted && !schedule.IsDeleted && !tour.IsDeleted
                  && o.UserId == userId
            select new
            {
                bookingId = b.Id,
                tourId = tour.Id,
                tourTitle = tour.Translations
                    .Where(tr => !tr.IsDeleted)
                    .OrderBy(tr => tr.Language == Language.English ? 0 : 1)
                    .Select(tr => tr.Title)
                    .FirstOrDefault() ?? "Tour",
                tourSlug = SlugHelper.ToUrlSlug(
                    tour.Translations
                        .Where(tr => !tr.IsDeleted)
                        .OrderBy(tr => tr.Language == Language.English ? 0 : 1)
                        .Select(tr => tr.Title)
                        .FirstOrDefault() ?? "tour"),
                dateIso = schedule.StartDate,
                startTime = slot.StartTime,
                paymentStatus = pay.Status,
                orderStatus = o.Status,
                bookingStatus = b.Status,
                paidAmount = pay.TotalAmount,
                currency = pay.Currency
            })
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(paymentStatus) && paymentStatus.Equals("paid", StringComparison.OrdinalIgnoreCase))
        {
            rows = rows
                .Where(x => x.paymentStatus == PaymentStatus.Completed && x.orderStatus == OrderStatus.Paid)
                .ToList();
        }

        return Result.Ok(rows);
    }

    [HttpPost("bookings/pay-now")]
    public async Task<IResultBase> PayNow([FromBody] PayNowRequest body)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));
        if (body is null || body.Lines is null || body.Lines.Count == 0)
            return Result.Fail("At least one booking line is required.");

        var currency = Enum.TryParse<Currency>(body.CurrencyCode, true, out var parsedCurrency)
            ? parsedCurrency
            : Currency.USD;

        var nowUtc = DateTime.UtcNow;
        var nowForTimestampWithoutTz = DateTime.SpecifyKind(nowUtc, DateTimeKind.Unspecified);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Currency = currency,
            Status = OrderStatus.Paid,
            OrderDate = nowForTimestampWithoutTz,
            TotalAmount = body.TotalAmount
        };
        await db.Orders.AddAsync(order);

        var validLineCount = 0;
        foreach (var line in body.Lines)
        {
            var slotId = line.SlotId;
            var slot = await db.AvailableSlots.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == slotId && !s.IsDeleted);
            if (slot is null)
                continue;

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                AvailableSlotsId = slotId,
                Price = line.LineTotal,
                Quantity = Math.Max(1, line.Quantity)
            };
            await db.OrderItems.AddAsync(orderItem);

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                AvailableSlotsId = slotId,
                Status = BookingStatus.Confirmed,
                BookingDate = nowForTimestampWithoutTz
            };
            await db.Bookings.AddAsync(booking);
            validLineCount++;
        }

        if (validLineCount == 0)
            return Result.Fail("No valid slots were found for this booking.");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            TotalAmount = body.TotalAmount,
            PaymentMethod = PaymentMethod.Card,
            Currency = currency,
            PaidAt = nowUtc,
            Status = PaymentStatus.Completed,
            TransactionId = $"SIM-{DateTime.UtcNow:yyyyMMddHHmmssfff}"
        };
        await db.Payments.AddAsync(payment);

        await db.SaveChangesAsync();
        return Result.Ok(new { orderId = order.Id, paymentStatus = "Completed" });
    }
}

public class FavoriteRequest
{
    public Guid TourId { get; set; }
}

public class PayNowRequest
{
    public string CurrencyCode { get; set; } = "USD";
    public decimal TotalAmount { get; set; }
    public List<PayNowLine> Lines { get; set; } = new();
}

public class PayNowLine
{
    public Guid SlotId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal LineTotal { get; set; }
}

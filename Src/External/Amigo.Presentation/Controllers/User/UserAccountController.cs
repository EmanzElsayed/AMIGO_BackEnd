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
            userType,
            emailConfirmed = appUser?.EmailConfirmed ?? false
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
                    .OrderBy(tr => tr.Language == Language.en ? 0 : 1)
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

   // [HttpGet("bookings")]
    //public async Task<IResultBase> GetBookings([FromQuery] string? paymentStatus = null)
    //{
    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //    if (string.IsNullOrWhiteSpace(userId))
    //        return Result.Fail(new UnauthorizedError("Not authenticated"));

    //    var rows = await (
    //        from b in db.Bookings.AsNoTracking()
    //        join o in db.Orders.AsNoTracking() on b.OrderId equals o.Id
    //        join pay in db.Payments.AsNoTracking() on o.Id equals pay.OrderId
    //        join slot in db.AvailableSlots.AsNoTracking() on b.AvailableSlotsId equals slot.Id
    //        join schedule in db.TourSchedules.AsNoTracking() on slot.TourScheduleId equals schedule.Id
    //        join tour in db.Tours.AsNoTracking() on schedule.TourId equals tour.Id
    //        where !b.IsDeleted && !o.IsDeleted && !pay.IsDeleted && !slot.IsDeleted && !schedule.IsDeleted && !tour.IsDeleted
    //              && o.UserId == userId
    //        select new
    //        {
    //            bookingId = b.Id,
    //            tourId = tour.Id,
    //            tourTitle = tour.Translations
    //                .Where(tr => !tr.IsDeleted)
    //                .OrderBy(tr => tr.Language == Language.English ? 0 : 1)
    //                .Select(tr => tr.Title)
    //                .FirstOrDefault() ?? "Tour",
    //            tourSlug = SlugHelper.ToUrlSlug(
    //                tour.Translations
    //                    .Where(tr => !tr.IsDeleted)
    //                    .OrderBy(tr => tr.Language == Language.English ? 0 : 1)
    //                    .Select(tr => tr.Title)
    //                    .FirstOrDefault() ?? "tour"),
    //            dateIso = schedule.StartDate,
    //            startTime = slot.StartTime,
    //            paymentStatus = pay.Status,
    //            orderStatus = o.Status,
    //            bookingStatus = b.Status,
    //            paidAmount = pay.TotalAmount,
    //            currency = pay.Currency
    //        })
    //        .ToListAsync();

    //    if (!string.IsNullOrWhiteSpace(paymentStatus) && paymentStatus.Equals("paid", StringComparison.OrdinalIgnoreCase))
    //    {
    //        rows = rows
    //            .Where(x => x.paymentStatus == PaymentStatus.Completed && x.orderStatus == OrderStatus.Paid)
    //            .ToList();
    //    }

    //    return Result.Ok(rows);
    //}

    //[HttpPost("bookings/pay-now")]
    //public async Task<IResultBase> PayNow([FromBody] PayNowRequest body)
    //{
    //    try
    //    {
    //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //        if (string.IsNullOrWhiteSpace(userId))
    //            return Result.Fail(new UnauthorizedError("Not authenticated"));

    //        var currentUser = await userManager.FindByIdAsync(userId);
    //        if (currentUser is null)
    //            return Result.Fail(new UnauthorizedError("Not authenticated"));
    //        if (!currentUser.EmailConfirmed)
    //            return Result.Fail(new ForbiddenError("Please confirm your email before booking."));

    //        if (body is null || body.Lines is null || body.Lines.Count == 0)
    //            return Result.Fail("At least one booking line is required.");

    //        if (body.Lines.Any(x => x.SlotId == Guid.Empty))
    //            return Result.Fail("Each booking line must include a valid slotId.");
    //        if (body.Lines.Any(x => x.Quantity < 1))
    //            return Result.Fail("Each booking line must have quantity >= 1.");
    //        if (body.Lines.Any(x => x.LineTotal < 0))
    //            return Result.Fail("Each booking line must have a non-negative lineTotal.");

    //        var slotIds = body.Lines.Select(x => x.SlotId).Distinct().ToList();
    //        var existingSlotIds = await db.AvailableSlots.AsNoTracking()
    //            .Where(s => slotIds.Contains(s.Id) && !s.IsDeleted)
    //            .Select(s => s.Id)
    //            .ToListAsync();
    //        if (existingSlotIds.Count != slotIds.Count)
    //            return Result.Fail("One or more selected slots are no longer available.");

    //        var requestedBySlot = body.Lines
    //            .GroupBy(x => x.SlotId)
    //            .ToDictionary(g => g.Key, g => g.Sum(x => Math.Max(1, x.Quantity)));

    //        var slotCapacities = await db.AvailableSlots.AsNoTracking()
    //            .Where(s => slotIds.Contains(s.Id) && !s.IsDeleted)
    //            .Select(s => new { s.Id, s.MaxCapacity })
    //            .ToDictionaryAsync(x => x.Id, x => x.MaxCapacity);

    //        var bookedBySlot = await (
    //            from item in db.OrderItems.AsNoTracking()
    //            join paidOrder in db.Orders.AsNoTracking() on item.OrderId equals paidOrder.Id
    //            where !item.IsDeleted
    //                && !paidOrder.IsDeleted
    //                && paidOrder.Status == OrderStatus.Paid
    //                && slotIds.Contains(item.AvailableSlotsId)
    //            group item by item.AvailableSlotsId into g
    //            select new { SlotId = g.Key, Qty = g.Sum(x => x.Quantity) }
    //        ).ToDictionaryAsync(x => x.SlotId, x => x.Qty);

    //        foreach (var slotId in slotIds)
    //        {
    //            if (!slotCapacities.TryGetValue(slotId, out var maxCapacity) || maxCapacity <= 0)
    //                return Result.Fail("Selected slot is not open for booking.");

    //            var alreadyBooked = bookedBySlot.TryGetValue(slotId, out var bq) ? bq : 0;
    //            var requestedQty = requestedBySlot.TryGetValue(slotId, out var rq) ? rq : 0;
    //            if (alreadyBooked + requestedQty > maxCapacity)
    //            {
    //                var remaining = Math.Max(0, maxCapacity - alreadyBooked);
    //                return Result.Fail($"Selected slot capacity exceeded. Remaining seats: {remaining}.");
    //            }
    //        }

    //        var currency = Enum.TryParse<CurrencyCode>(body.CurrencyCode, true, out var parsedCurrency)
    //            ? parsedCurrency
    //            : CurrencyCode.USD;

    //        var nowUtc = DateTime.UtcNow;
    //        var nowNoTz = DateTime.SpecifyKind(nowUtc, DateTimeKind.Unspecified);
    //        var totalAmount = body.TotalAmount > 0
    //            ? body.TotalAmount
    //            : body.Lines.Sum(x => Math.Max(0, x.LineTotal));

    //        var order = new Order
    //        {
    //            Id = Guid.NewGuid(),
    //            UserId = userId,
    //            Currency = currency,
    //            Status = OrderStatus.Paid,
    //            OrderDate = nowUtc,
    //            TotalAmount = totalAmount
    //        };
    //        await db.Orders.AddAsync(order);

    //        foreach (var line in body.Lines)
    //        {
    //            var orderItem = new OrderItem
    //            {
    //                Id = Guid.NewGuid(),
    //                OrderId = order.Id,
    //                AvailableSlotsId = line.SlotId,
    //                Price = Math.Max(0, line.LineTotal),
    //                Quantity = Math.Max(1, line.Quantity)
    //            };
    //            await db.OrderItems.AddAsync(orderItem);

    //            var booking = new Booking
    //            {
    //                Id = Guid.NewGuid(),
    //                OrderId = order.Id,
    //                AvailableSlotsId = line.SlotId,
    //                Status = BookingStatus.Confirmed,
    //                ConfirmedAt = nowNoTz
    //            };
    //            await db.Bookings.AddAsync(booking);
    //        }

    //        var payment = new Payment
    //        {
    //            Id = Guid.NewGuid(),
    //            OrderId = order.Id,
    //            TotalAmount = totalAmount,
    //            PaymentMethod = PaymentMethod.Card,
    //            Currency = currency,
    //            PaidAt = nowUtc,
    //            Status = PaymentStatus.Completed,
    //            ProviderTransactionId = $"SIM-{DateTime.UtcNow:yyyyMMddHHmmssfff}"
    //        };
    //        await db.Payments.AddAsync(payment);

    //        await db.SaveChangesAsync();
    //        return Result.Ok(new { orderId = order.Id, paymentStatus = "Completed" });
    //    }
    //    catch (DbUpdateException ex)
    //    {
    //        return Result.Fail($"Checkout save failed: {ex.InnerException?.Message ?? ex.Message}");
    //    }
    //}
}

public class FavoriteRequest
{
    public Guid TourId { get; set; }
}

//public class PayNowRequest
//{
//    public string CurrencyCode { get; set; } = "USD";
//    public decimal TotalAmount { get; set; }
//    public string FullName { get; set; } = "";
//    public string Email { get; set; } = "";
//    public string PhoneNumber { get; set; } = "";
//    public List<PayNowLine> Lines { get; set; } = new();
//}

//public class PayNowLine
//{
//    public Guid SlotId { get; set; }
//    public int Quantity { get; set; } = 1;
//    public decimal LineTotal { get; set; }
//}

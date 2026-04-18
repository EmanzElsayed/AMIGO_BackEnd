//using Amigo.Domain.Entities;
//using Amigo.Domain.Enum;
//using Amigo.Persistence;
//using Amigo.SharedKernal.DTOs.Results;
//using FluentResults;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.EntityFrameworkCore;

//namespace Amigo.Presentation.Controllers.Admin;

//[Route("api/v1/admin/activity")]
//[Authorize(Roles = "Admin")]
//public class AdminActivityController(AmigoDbContext db) : BaseController
//{
//    [HttpGet]
//    public async Task<IResultBase> GetActivities(
//        [FromQuery] string? Name,
//         string? DestinationId,
//        int PageNumber = 1,
//       int PageSize = 10)
//    {
//        var pn = PageNumber < 1 ? 1 : PageNumber;
//        var ps = PageSize is < 1 or > 100 ? 48 : PageSize;

//        Guid? destId = null;
//        if (!string.IsNullOrWhiteSpace(DestinationId) && Guid.TryParse(DestinationId, out var g) && g != Guid.Empty)
//            destId = g;

//        var q = db.Tours.AsNoTracking().Where(t => !t.IsDeleted);

//        if (destId.HasValue)
//            q = q.Where(t => t.DestinationId == destId.Value);

//        if (!string.IsNullOrWhiteSpace(Name))
//        {
//            var term = Name.Trim();
//            var pattern = $"%{EscapeLikePattern(term)}%";
//            q = q.Where(t => t.Translations.Any(tr => EF.Functions.ILike(tr.Title, pattern)));
//        }

//        var totalItems = await q.CountAsync();

//        var rows = await q
//            .OrderByDescending(t => t.CreatedDate)
//            .Skip((pn - 1) * ps)
//            .Take(ps)
//            .Select(t => new AdminActivityListItemDto
//            {
//                ActivityId = t.Id,
//                TourId = t.Id,
//                Title = t.Translations.Where(x => x.Language == Language.English).Select(x => x.Title).FirstOrDefault()
//                    ?? t.Translations.Select(x => x.Title).FirstOrDefault(),
//                DestinationName = t.Destination.Translations.Where(x => x.Language == Language.English).Select(x => x.Name).FirstOrDefault()
//                    ?? t.Destination.Translations.Select(x => x.Name).FirstOrDefault(),
//                ImageUrl = t.Images.Select(i => i.ImageUrl).FirstOrDefault(),
//                EntryAmountVIP = t.Prices
//                    .Where(p => !p.IsDeleted && p.UserType == UserType.VIP)
//                    .OrderByDescending(p => p.Cost * (1 - p.Discount / 100m))
//                    .Select(p => (decimal?)(p.Cost * (1 - p.Discount / 100m)))
//                    .FirstOrDefault(),
//                EntryAmountVIPLabel = t.Prices
//                    .Where(p => !p.IsDeleted && p.UserType == UserType.VIP)
//                    .OrderByDescending(p => p.Cost * (1 - p.Discount / 100m))
//                    .Select(p => p.Translations
//                        .Where(tr => tr.Language == Language.English)
//                        .Select(tr => tr.Type)
//                        .FirstOrDefault()
//                        ?? p.Translations.Select(tr => tr.Type).FirstOrDefault())
//                    .FirstOrDefault(),
//                EntryAmountPublic = t.Prices
//                    .Where(p => !p.IsDeleted && p.UserType == UserType.Public)
//                    .OrderByDescending(p => p.Cost * (1 - p.Discount / 100m))
//                    .Select(p => (decimal?)(p.Cost * (1 - p.Discount / 100m)))
//                    .FirstOrDefault(),
//                EntryAmountPublicLabel = t.Prices
//                    .Where(p => !p.IsDeleted && p.UserType == UserType.Public)
//                    .OrderByDescending(p => p.Cost * (1 - p.Discount / 100m))
//                    .Select(p => p.Translations
//                        .Where(tr => tr.Language == Language.English)
//                        .Select(tr => tr.Type)
//                        .FirstOrDefault()
//                        ?? p.Translations.Select(tr => tr.Type).FirstOrDefault())
//                    .FirstOrDefault(),
//                TotalCapacity = db.AvailableSlots
//                    .Where(s => !s.IsDeleted && s.TourSchedule.TourId == t.Id)
//                    .Select(s => (int?)s.MaxCapacity)
//                    .Sum() ?? 0,
//                BookedSeats = (
//                    from oi in db.OrderItems
//                    join o in db.Orders on oi.OrderId equals o.Id
//                    join s in db.AvailableSlots on oi.AvailableSlotsId equals s.Id
//                    where !oi.IsDeleted
//                        && !o.IsDeleted
//                        && !s.IsDeleted
//                        && o.Status == OrderStatus.Paid
//                        && s.TourSchedule.TourId == t.Id
//                    select (int?)oi.Quantity
//                ).Sum() ?? 0,
//            })
//            .ToListAsync();

//        foreach (var item in rows)
//        {
//            if (item.TotalCapacity <= 0)
//            {
//                item.BookedPercentage = 0;
//                item.Status = "Active";
//                continue;
//            }

//            var ratio = (decimal)item.BookedSeats / item.TotalCapacity;
//            var pct = (int)Math.Round(ratio * 100m, MidpointRounding.AwayFromZero);
//            item.BookedPercentage = Math.Clamp(pct, 0, 100);
//            item.Status = item.BookedPercentage >= 90 ? "Low Stock" : "Active";
//        }

//        var totalPages = ps <= 0 ? 0 : (int)Math.Ceiling(totalItems / (double)ps);

//        var response = new PaginatedResponse<AdminActivityListItemDto>
//        {
//            Data = rows,
//            PageNumber = pn,
//            PageSize = ps,
//            TotalItems = totalItems,
//            TotalPages = totalPages,
//        };

//        return Result.Ok(response);
//    }

//    [HttpGet("stats")]
//    public async Task<IResultBase> GetActivityStats()
//    {
//        var now = DateTime.UtcNow;
//        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
//        var nextMonthStart = monthStart.AddMonths(1);

//        var bookingsThisMonth = await db.Bookings.AsNoTracking()
//            .Where(b => !b.IsDeleted && b.ConfirmedAt.HasValue && b.ConfirmedAt.Value >= monthStart && b.ConfirmedAt.Value < nextMonthStart)
//            .CountAsync();

//        var grossRevenue = await db.Orders.AsNoTracking()
//            .Where(o => !o.IsDeleted && o.Status == OrderStatus.Paid && o.OrderDate.HasValue && o.OrderDate.Value >= monthStart && o.OrderDate.Value < nextMonthStart)
//            .Select(o => (decimal?)o.TotalAmount)
//            .SumAsync() ?? 0m;

//        var totalCapacity = await db.AvailableSlots.AsNoTracking()
//            .Where(s => !s.IsDeleted)
//            .Select(s => (int?)s.MaxCapacity)
//            .SumAsync() ?? 0;

//        var totalBookedSeats = await (
//            from oi in db.OrderItems.AsNoTracking()
//            join o in db.Orders.AsNoTracking() on oi.OrderId equals o.Id
//            where !oi.IsDeleted && !o.IsDeleted && o.Status == OrderStatus.Paid
//            select (int?)oi.Quantity
//        ).SumAsync() ?? 0;

//        var avgCapacity = totalCapacity <= 0
//            ? 0
//            : Math.Clamp((int)Math.Round((decimal)totalBookedSeats * 100m / totalCapacity, MidpointRounding.AwayFromZero), 0, 100);

//        var status = avgCapacity >= 90 ? "Low Stock" : "Active";

//        return Result.Ok(new
//        {
//            bookingsThisMonth,
//            avgCapacity,
//            grossRevenue,
//            status
//        });
//    }

//    private static string EscapeLikePattern(string value)
//    {
//        return value
//            .Replace("\\", "\\\\")
//            .Replace("%", "\\%")
//            .Replace("_", "\\_");
//    }
//}

//public sealed class AdminActivityListItemDto
//{
//    public Guid ActivityId { get; set; }
//    public Guid TourId { get; set; }
//    public string? Title { get; set; }
//    public string? DestinationName { get; set; }
//    public string? ImageUrl { get; set; }
//    public decimal? EntryAmountVIP { get; set; }
//    public string? EntryAmountVIPLabel { get; set; }
//    public decimal? EntryAmountPublic { get; set; }
//    public string? EntryAmountPublicLabel { get; set; }
//    public int TotalCapacity { get; set; }
//    public int BookedSeats { get; set; }
//    public int BookedPercentage { get; set; }
//    public string Status { get; set; } = "Active";
//}

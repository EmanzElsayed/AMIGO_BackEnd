using Amigo.Domain.Entities.Identity;
using Amigo.Domain.Enum;
using Amigo.Persistence;
using Amigo.Presentation.Specifications.Admin;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Amigo.Presentation.Controllers.Admin;

[Route("api/v1/admin/customer")]
[Authorize(Roles = "Admin")]
public class AdminCustomerController(
    AmigoDbContext db,
    UserManager<ApplicationUser> userManager) : BaseController
{
    [HttpGet]
    //public async Task<IResultBase> GetCustomers([FromQuery] AdminCustomersQuery filters)
    //{
    //    var pageNumber = filters.PageNumber < 1 ? 1 : filters.PageNumber;
    //    var pageSize = filters.PageSize is < 1 or > 50 ? 8 : filters.PageSize;
    //    var searchText = FirstNonEmpty(filters.Query, filters.Q, filters.Search);
    //    var countryFilter = FirstNonEmpty(filters.Country, filters.CountryCode);

    //    var adminRoleId = await db.Roles.AsNoTracking()
    //        .Where(r => r.NormalizedName == "ADMIN")
    //        .Select(r => r.Id)
    //        .FirstOrDefaultAsync();

    //    var adminUserIds = string.IsNullOrWhiteSpace(adminRoleId)
    //        ? new HashSet<string>()
    //        : (await db.UserRoles.AsNoTracking()
    //            .Where(ur => ur.RoleId == adminRoleId)
    //            .Select(ur => ur.UserId)
    //            .Distinct()
    //            .ToListAsync())
    //            .ToHashSet();

    //    filters.Query = searchText;
    //    filters.Country = countryFilter;
    //    var filterSpec = new AdminCustomersFilterSpecification(filters, adminUserIds.ToList());
    //    var usersQ = db.Users.AsNoTracking().Where(filterSpec.Criteria);
    //    var totalCount = await usersQ.CountAsync();
    //    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

    //    var pagedUsers = await usersQ
    //        .OrderByDescending(u => u.CreatedDate)
    //        .Skip((pageNumber - 1) * pageSize)
    //        .Take(pageSize)
    //        .Select(u => new
    //        {
    //            u.Id,
    //            u.FullName,
    //            u.Email,
    //            u.PhoneNumber,
    //            u.Image,
    //            u.IsActive,
    //            u.CreatedDate,
    //            Country = u.Address != null ? u.Address.Country : null,
    //        })
    //        .ToListAsync();

    //    var userIds = pagedUsers.Select(x => x.Id).ToList();

    //    var vipRoleId = await db.Roles.AsNoTracking()
    //        .Where(r => r.NormalizedName == "VIP")
    //        .Select(r => r.Id)
    //        .FirstOrDefaultAsync();

    //    var vipUserIds = string.IsNullOrWhiteSpace(vipRoleId)
    //        ? []
    //        : await db.UserRoles.AsNoTracking()
    //            .Where(ur => ur.RoleId == vipRoleId && userIds.Contains(ur.UserId))
    //            .Select(ur => ur.UserId)
    //            .ToListAsync();

    //    var bookingsByUser = await (
    //            from b in db.Bookings.AsNoTracking()
    //            join o in db.Orders.AsNoTracking() on b.OrderId equals o.Id
    //            where !b.IsDeleted && !o.IsDeleted && userIds.Contains(o.UserId)
    //            group b by o.UserId into g
    //            select new { UserId = g.Key, Count = g.Count() }
    //        )
    //        .ToDictionaryAsync(x => x.UserId, x => x.Count);

    //    var spendByUser = await (
    //            from p in db.Payments.AsNoTracking()
    //            join o in db.Orders.AsNoTracking() on p.OrderId equals o.Id
    //            where !p.IsDeleted && !o.IsDeleted
    //                  && p.Status == PaymentStatus.Completed
    //                  && userIds.Contains(o.UserId)
    //            group p by o.UserId into g
    //            select new { UserId = g.Key, Amount = g.Sum(x => x.TotalAmount) }
    //        )
    //        .ToDictionaryAsync(x => x.UserId, x => x.Amount);

    //    var items = pagedUsers.Select(u =>
    //    {
    //        var booked = bookingsByUser.TryGetValue(u.Id, out var c) ? c : 0;
    //        var spend = spendByUser.TryGetValue(u.Id, out var s) ? s : 0m;
    //        var country = string.IsNullOrWhiteSpace(u.Country) ? "—" : u.Country!;
    //        var since = u.CreatedDate.Year < 2001 ? DateTime.UtcNow : u.CreatedDate;
    //        return new
    //        {
    //            id = u.Id,
    //            customerCode = $"CUST-{u.Id[..Math.Min(4, u.Id.Length)].ToUpper()}",
    //            fullName = u.FullName ?? "User",
    //            avatarUrl = u.Image,
    //            email = u.Email ?? "",
    //            phoneNumber = u.PhoneNumber,
    //            country,
    //            since = since.ToString("MMM dd, yyyy"),
    //            bookings = booked,
    //            spend,
    //            status = u.IsActive ? "active" : "inactive",
    //            isVip = vipUserIds.Contains(u.Id),
    //            userType = vipUserIds.Contains(u.Id) ? "VIP" : "Public",
    //            isAdmin = false,
    //        };
    //    }).ToList();

    //    var vipMembers = 0;
    //    if (!string.IsNullOrWhiteSpace(vipRoleId))
    //    {
    //        var vipMembersQuery = db.UserRoles.AsNoTracking().Where(ur => ur.RoleId == vipRoleId);
    //        if (adminUserIds.Count > 0)
    //            vipMembersQuery = vipMembersQuery.Where(ur => !adminUserIds.Contains(ur.UserId));
    //        vipMembers = await vipMembersQuery
    //            .Select(ur => ur.UserId)
    //            .Distinct()
    //            .CountAsync();
    //    }

    //    var now = DateTime.UtcNow;
    //    var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
    //    var nextMonthStart = currentMonthStart.AddMonths(1);
    //    var previousMonthStart = currentMonthStart.AddMonths(-1);

    //    var currentMonthQ = db.Users.AsNoTracking()
    //        .Where(u => !u.IsDeleted && u.CreatedDate >= currentMonthStart && u.CreatedDate < nextMonthStart);
    //    if (adminUserIds.Count > 0)
    //        currentMonthQ = currentMonthQ.Where(u => !adminUserIds.Contains(u.Id));
    //    var currentMonthCustomers = await currentMonthQ.CountAsync();

    //    var previousMonthQ = db.Users.AsNoTracking()
    //        .Where(u => !u.IsDeleted && u.CreatedDate >= previousMonthStart && u.CreatedDate < currentMonthStart);
    //    if (adminUserIds.Count > 0)
    //        previousMonthQ = previousMonthQ.Where(u => !adminUserIds.Contains(u.Id));
    //    var previousMonthCustomers = await previousMonthQ.CountAsync();

    //    decimal growthPercent;
    //    if (previousMonthCustomers <= 0)
    //    {
    //        growthPercent = currentMonthCustomers > 0 ? 100m : 0m;
    //    }
    //    else
    //    {
    //        growthPercent = ((currentMonthCustomers - previousMonthCustomers) / (decimal)previousMonthCustomers) * 100m;
    //    }
    //    var growthText = $"{(growthPercent >= 0 ? "+" : "")}{Math.Round(growthPercent, 1):0.#}%";

    //    return Result.Ok(new
    //    {
    //        items,
    //        totalCount,
    //        totalPages = totalPages < 1 ? 1 : totalPages,
    //        totalCustomers = totalCount,
    //        vipMembers,
    //        growthText,
    //    });
    //}

    [HttpPatch("{id}/vip-status")]
    public async Task<IResultBase> UpdateVipStatus([FromRoute] string id, [FromBody] UpdateVipStatusRequest body)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Result.Fail("Customer id is required.");

        var user = await userManager.FindByIdAsync(id);
        if (user is null || user.IsDeleted)
            return Result.Fail("Customer not found.");
        if (await userManager.IsInRoleAsync(user, "Admin"))
            return Result.Fail("Cannot change VIP for admin users.");

        var hasVip = await userManager.IsInRoleAsync(user, "VIP");
        if (body.IsVip && !hasVip)
            await userManager.AddToRoleAsync(user, "VIP");
        if (!body.IsVip && hasVip)
            await userManager.RemoveFromRoleAsync(user, "VIP");

        return Result.Ok(new { id, isVip = body.IsVip });
    }

    private static string? FirstNonEmpty(params string?[] values)
    {
        foreach (var v in values)
        {
            if (!string.IsNullOrWhiteSpace(v))
                return v;
        }
        return null;
    }

}

public class AdminCustomersQuery
{
    public string? Query { get; set; }
    public string? Q { get; set; }
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? Country { get; set; }
    public string? CountryCode { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 8;
}

public class UpdateVipStatusRequest
{
    public bool IsVip { get; set; }
}

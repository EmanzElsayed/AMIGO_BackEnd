using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Amigo.Persistence;
using Amigo.SharedKernal.DTOs.Results;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Amigo.Presentation.Controllers.Admin;

[Route("api/v1/admin/activity")]
[Authorize(Roles = "Admin")]
public class AdminActivityController(AmigoDbContext db) : BaseController
{
    [HttpGet]
    public async Task<IResultBase> GetActivities(
        [FromQuery] string? Name,
         string? DestinationId,
        int PageNumber = 1,
       int PageSize = 10)
    {
        var pn = PageNumber < 1 ? 1 : PageNumber;
        var ps = PageSize is < 1 or > 100 ? 48 : PageSize;

        Guid? destId = null;
        if (!string.IsNullOrWhiteSpace(DestinationId) && Guid.TryParse(DestinationId, out var g) && g != Guid.Empty)
            destId = g;

        var q = db.Tours.AsNoTracking().Where(t => !t.IsDeleted);

        if (destId.HasValue)
            q = q.Where(t => t.DestinationId == destId.Value);

        if (!string.IsNullOrWhiteSpace(Name))
        {
            var term = Name.Trim();
            var pattern = $"%{EscapeLikePattern(term)}%";
            q = q.Where(t => t.Translations.Any(tr => EF.Functions.ILike(tr.Title, pattern)));
        }

        var totalItems = await q.CountAsync();

        var rows = await q
            .OrderByDescending(t => t.CreatedDate)
            .Skip((pn - 1) * ps)
            .Take(ps)
            .Select(t => new AdminActivityListItemDto
            {
                ActivityId = t.Id,
                TourId = t.Id,
                Title = t.Translations.Where(x => x.Language == Language.English).Select(x => x.Title).FirstOrDefault()
                    ?? t.Translations.Select(x => x.Title).FirstOrDefault(),
                DestinationName = t.Destination.Translations.Where(x => x.Language == Language.English).Select(x => x.Name).FirstOrDefault()
                    ?? t.Destination.Translations.Select(x => x.Name).FirstOrDefault(),
                ImageUrl = t.Images.Select(i => i.ImageUrl).FirstOrDefault(),
                EntryAmountVIP = t.Prices
                    .Where(p => !p.IsDeleted && p.UserType == UserType.VIP)
                    .OrderByDescending(p => p.Cost * (1 - p.Discount / 100m))
                    .Select(p => (decimal?)(p.Cost * (1 - p.Discount / 100m)))
                    .FirstOrDefault(),
                EntryAmountVIPLabel = t.Prices
                    .Where(p => !p.IsDeleted && p.UserType == UserType.VIP)
                    .OrderByDescending(p => p.Cost * (1 - p.Discount / 100m))
                    .Select(p => p.Translations
                        .Where(tr => tr.Language == Language.English)
                        .Select(tr => tr.Type)
                        .FirstOrDefault()
                        ?? p.Translations.Select(tr => tr.Type).FirstOrDefault())
                    .FirstOrDefault(),
                EntryAmountPublic = t.Prices
                    .Where(p => !p.IsDeleted && p.UserType == UserType.Public)
                    .OrderByDescending(p => p.Cost * (1 - p.Discount / 100m))
                    .Select(p => (decimal?)(p.Cost * (1 - p.Discount / 100m)))
                    .FirstOrDefault(),
                EntryAmountPublicLabel = t.Prices
                    .Where(p => !p.IsDeleted && p.UserType == UserType.Public)
                    .OrderByDescending(p => p.Cost * (1 - p.Discount / 100m))
                    .Select(p => p.Translations
                        .Where(tr => tr.Language == Language.English)
                        .Select(tr => tr.Type)
                        .FirstOrDefault()
                        ?? p.Translations.Select(tr => tr.Type).FirstOrDefault())
                    .FirstOrDefault(),
                Status = "Active",
            })
            .ToListAsync();

        var totalPages = ps <= 0 ? 0 : (int)Math.Ceiling(totalItems / (double)ps);

        var response = new PaginatedResponse<AdminActivityListItemDto>
        {
            Data = rows,
            PageNumber = pn,
            PageSize = ps,
            TotalItems = totalItems,
            TotalPages = totalPages,
        };

        return Result.Ok(response);
    }

    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
    }
}

public sealed class AdminActivityListItemDto
{
    public Guid ActivityId { get; set; }
    public Guid TourId { get; set; }
    public string? Title { get; set; }
    public string? DestinationName { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? EntryAmountVIP { get; set; }
    public string? EntryAmountVIPLabel { get; set; }
    public decimal? EntryAmountPublic { get; set; }
    public string? EntryAmountPublicLabel { get; set; }
    public string Status { get; set; } = "Active";
}

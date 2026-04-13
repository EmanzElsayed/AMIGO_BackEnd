using Amigo.Domain.Entities.Identity;
using Amigo.Presentation.Controllers.Admin;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Amigo.Presentation.Specifications.Admin;

internal sealed class AdminCustomersFilterSpecification
{
    public AdminCustomersFilterSpecification(
        AdminCustomersQuery query,
        IReadOnlyCollection<string> adminUserIds)
    {
        Criteria = BuildCriteria(query, adminUserIds);
    }

    public Expression<Func<ApplicationUser, bool>> Criteria { get; }

    private static Expression<Func<ApplicationUser, bool>> BuildCriteria(
        AdminCustomersQuery query,
        IReadOnlyCollection<string> adminUserIds)
    {
        var status = (query.Status ?? string.Empty).Trim().ToLowerInvariant();
        var country = FirstNonEmpty(query.Country, query.CountryCode)?.Trim();
        var search = FirstNonEmpty(query.Query, query.Q, query.Search)?.Trim();

        var hasAdminFilter = adminUserIds.Count > 0;
        var hasStatusFilter = !string.IsNullOrWhiteSpace(status) && status != "all";
        var hasCountryFilter = !string.IsNullOrWhiteSpace(country);
        var hasSearchFilter = !string.IsNullOrWhiteSpace(search);

        var countryPattern = hasCountryFilter ? $"%{EscapeLikePattern(country!)}%" : string.Empty;
        var searchPattern = hasSearchFilter ? $"%{EscapeLikePattern(search!)}%" : string.Empty;
        var normalizedEmail = hasSearchFilter ? search!.Replace(" ", "").ToUpperInvariant() : string.Empty;

        return u =>
            !u.IsDeleted
            && (!hasAdminFilter || !adminUserIds.Contains(u.Id))
            && (!hasStatusFilter
                || (status == "active" ? u.IsActive : !u.IsActive))
            && (!hasCountryFilter
                || (u.Address != null && u.Address.Country != null && EF.Functions.ILike(u.Address.Country, countryPattern))
                || (u.Address != null && u.Address.City != null && EF.Functions.ILike(u.Address.City, countryPattern))
                || (u.Nationality != null && EF.Functions.ILike(u.Nationality, countryPattern)))
            && (!hasSearchFilter
                || (u.FullName != null && EF.Functions.ILike(u.FullName, searchPattern))
                || (u.Email != null && EF.Functions.ILike(u.Email, searchPattern))
                || (u.NormalizedEmail != null && u.NormalizedEmail.Contains(normalizedEmail))
                || (u.PhoneNumber != null && EF.Functions.ILike(u.PhoneNumber, searchPattern))
                || EF.Functions.ILike(u.Id, searchPattern));
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

    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
    }
}


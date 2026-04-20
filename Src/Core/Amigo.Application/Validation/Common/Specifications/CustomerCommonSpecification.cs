using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Amigo.Application.Validation.Common.Specifications;

public static class CustomerCommonSpecification
{
    public static Expression<Func<ApplicationUser, bool>> BuildCriteria
        ( GetAllCustomersQuery filters , List<string> adminIds)
    {
        var search = Normalize(filters.Query, filters.Q, filters.Search);
        var country = Normalize(filters.Country, filters.CountryCode);
        var status = filters.Status?.Trim().ToLower();

        return user  => 
           !user.IsDeleted &&

           user.EmailConfirmed &&
           !adminIds.Contains(user.Id)

           &&
           // Search
           (
               string.IsNullOrWhiteSpace(search) ||
               (user.FullName != null && user.FullName.Contains(search)) ||
               (user.Email != null && user.Email.Contains(search)) ||
               (user.PhoneNumber != null && user.PhoneNumber.Contains(search))
           ) &&

           // Status
           (
               string.IsNullOrWhiteSpace(status) ||
               (status == "active" && user.IsActive) ||
               (status == "inactive" && !user.IsActive)
           ) &&

           // Country
           (
               string.IsNullOrWhiteSpace(country) ||
               (user.Address != null &&
                user.Address.Country != null &&
                user.Address.Country == country)
           );

    }

    private static string? Normalize(params string?[] values)
    {
        return values.FirstOrDefault(x =>
            !string.IsNullOrWhiteSpace(x))?.Trim();
    }
}
 

using Amigo.Application.Abstraction.Services;
using Amigo.Domain.Entities.Identity;
using Amigo.SharedKernal.DTOs.Tour;
using Amigo.SharedKernal.QueryParams;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Amigo.Presentation.Controllers.User;

[Route("api/v1/user/tour")]
public class UserTourController(
                    IUserTourCatalogService _catalog,
                    ICheckoutQuoteService _checkoutQuote,
                    UserManager<ApplicationUser> _userManager)
                     : BaseController
{
    [HttpGet]
    public async Task<IResultBase> GetTours([FromQuery] GetUserToursQuery query)
    {
        var userType = await ResolveEffectiveUserTypeAsync();
        query.UserType = userType;
        return await _catalog.GetToursAsync(query);
    }



    [HttpGet("by-public-path")]
    public async Task<IResultBase> GetTourByPublicPath([FromQuery] GetTourByPublicPathQuery query)
    {
        var userType = await ResolveEffectiveUserTypeAsync();
        return await _catalog.GetTourByPublicPathAsync(query, userType);
    }

    [HttpGet("categories")]
    public async Task<IResultBase> GetCategories([FromQuery] Guid destinationId, [FromQuery] string? language)
    {
        return await _catalog.GetTourCategoriesAsync(destinationId, language);
    }

    [HttpGet("max-duration-hours")]
    public async Task<IResultBase> GetMaxDurationHours([FromQuery] Guid destinationId)
    {
        return await _catalog.GetMaxDurationHoursForDestinationAsync(destinationId);
    }

    [HttpGet("trending")]
    public async Task<IResultBase> GetTrendingTours([FromQuery] string? language, [FromQuery] string? currency, [FromQuery] int take = 6)
    {
        var userType = await ResolveEffectiveUserTypeAsync();
        return await _catalog.GetTrendingToursAsync(language, currency, userType, take);
    }

    [HttpPost("checkout/quote")]
    public async Task<IResultBase> PostCheckoutQuote([FromBody] CheckoutQuoteRequestDto body)
    {
        body = body with { EffectiveUserType = await ResolveEffectiveUserTypeAsync() };
        return await _checkoutQuote.QuoteAsync(body);
    }

    private async Task<string> ResolveEffectiveUserTypeAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return "Public";

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return "Public";

        var isVip = await _userManager.IsInRoleAsync(user, "VIP");
        return isVip ? "VIP" : "Public";
    }

}

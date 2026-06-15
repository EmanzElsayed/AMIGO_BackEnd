using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Price;
using Amigo.Domain.Entities.Identity;
using Amigo.Presentation.Attributes;
using Amigo.SharedKernal.DTOs.Tour;
using Amigo.SharedKernal.QueryParams;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;

using System.Security.Claims;

namespace Amigo.Presentation.Controllers.User;

[Route("api/v1/user/tour")]
public class UserTourController(

                    IServiceManager _serviceManager,
                    UserManager<ApplicationUser> _userManager,
                    Microsoft.Extensions.Logging.ILogger<UserTourController> _logger)
                     : BaseController
{
    [EnableRateLimiting("token")]

    [HttpGet]
    //[Cache(1800)]

    //updated done
    public async Task<IResultBase> GetTours([FromQuery] GetUserToursQuery query)
    {
        _logger.LogInformation("GetTours called: DestinationId={DestinationId}, AvailabilityDate={AvailabilityDate}, Language={Language}, Page={PageNumber}, Size={PageSize}", query.DestinationId, query.AvailabilityDate, query.Language, query.PageNumber, query.PageSize);
        var userType = await ResolveEffectiveUserTypeAsync();
        query.UserType = userType;
        _logger.LogInformation("GetTours forwarding to service with UserType={UserTypeEffective}", userType);
        return await _serviceManager.UserTourCatalogService.GetToursAsync(query);
    }


    [EnableRateLimiting("token")]

    [HttpGet("by-public-path")]
    //[Cache(1800)]
    public async Task<IResultBase> GetTourByPublicPath([FromQuery] GetTourByPublicPathQuery query)
    {
        var userType = await ResolveEffectiveUserTypeAsync();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _serviceManager.UserTourCatalogService.GetTourByPublicPathAsync(query, userType, userId);
    }

    [EnableRateLimiting("token")]

    [HttpGet("{id}/review")]
    //[Cache(1800)]
    public async Task<IResultBase> GetReviwsInfoByTourId(string id)
    {
        var userType = await ResolveEffectiveUserTypeAsync();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _serviceManager.UserTourCatalogService.GetTourReviews(id, userId);
    }


    [EnableRateLimiting("token")]

    [HttpGet("{id}/schedule-info")]
    //[Cache(1800)]
    public async Task<IResultBase> GetScheduleInfoByTourId(string id)
    {
        var userType = await ResolveEffectiveUserTypeAsync();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _serviceManager.UserTourCatalogService.GetTourScheduleDetails(id,userType);
    }


    [HttpGet("{id}/price-with-activity-type")]
    public async Task<IResultBase> GetPriceByActivityType(string id ,[FromQuery] PiceWithActivityTypeRequestQuery requestDTO)
    {
        var userType = await ResolveEffectiveUserTypeAsync();
        return await _serviceManager.UserTourCatalogService.GetPriceByActivityTypeAsync(id,requestDTO, userType);
    }


    [EnableRateLimiting("token")]

    [HttpGet("categories")]
    //[Cache(1800)]
    public async Task<IResultBase> GetCategories([FromQuery] Guid destinationId, [FromQuery] string? language)
    {
        return await _serviceManager.UserTourCatalogService.GetTourCategoriesAsync(destinationId, language);
    }
    [EnableRateLimiting("token")]

    [HttpGet("max-duration-hours")]
    //[Cache(1800)]

    public async Task<IResultBase> GetMaxDurationHours([FromQuery] Guid destinationId)
    {
        return await _serviceManager.UserTourCatalogService.GetMaxDurationHoursForDestinationAsync(destinationId);
    }
    [EnableRateLimiting("token")]

    [HttpGet("max-price")]
    //[Cache(900)]

    public async Task<IResultBase> GetMaxPrice([FromQuery] Guid destinationId)
    {
        return await _serviceManager.UserTourCatalogService.GetMaxPriceForDestinationAsync(destinationId);
    }
    [EnableRateLimiting("token")]

    [HttpGet("trending")]
    //[Cache(1800)]

    public async Task<IResultBase> GetTrendingTours([FromQuery] string? language, [FromQuery] string? currency, [FromQuery] int take = 6)
    {
        var userType = await ResolveEffectiveUserTypeAsync();
        return await _serviceManager.UserTourCatalogService.GetTrendingToursAsync(language, currency, userType, take);
    }
    [EnableRateLimiting("token")]

    [HttpPost("checkout/quote")]
    public async Task<IResultBase> PostCheckoutQuote([FromBody] CheckoutQuoteRequestDto body)
    {
        body = body with { EffectiveUserType = await ResolveEffectiveUserTypeAsync() };
        return await _serviceManager.CheckoutQuoteService.QuoteAsync(body);
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

using Amigo.Application.Abstraction.Services;
using Amigo.Domain.Entities.Identity;
using Amigo.Domain.Errors.BusinessErrors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Amigo.Presentation.Controllers.User
{
    [Route("api/v1/user/country")]

    public class CountryController(IServiceManager _serviceManager , UserManager<ApplicationUser> _userManager) : BaseController

    {
        [EnableRateLimiting("token")]
        [HttpGet("by-slug/{slug}")]
        public async Task<IResultBase> GetCountryBySlug(string slug, CancellationToken cancellationToken)
        {
            var id = await _serviceManager.DestinationSlugResolver.ResolveCountryIdAsync(slug, cancellationToken);
            if (id is null)
                return Result.Fail(new NotFoundError("Country not found for this link."));
            var userType = await ResolveEffectiveUserTypeAsync();

            return await _serviceManager.DestinationService.GetCountryByIdAsync(id.Value.ToString(), userType, cancellationToken);
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
}

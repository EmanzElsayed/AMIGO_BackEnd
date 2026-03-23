using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Amigo.Infrastructure.Localization;

public class HeaderRequestCultureProvider: RequestCultureProvider
{
    public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var acceptedLanguageHeader = httpContext.Request.Headers["Accept-Language"].ToString();
        if (string.IsNullOrWhiteSpace(acceptedLanguageHeader))
            return Task.FromResult<ProviderCultureResult?>(null);
        var firstLang = acceptedLanguageHeader.Split(',')[0];

        if(string.IsNullOrEmpty(firstLang))
            return Task.FromResult<ProviderCultureResult?>(null);
        return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(firstLang));

    }
}

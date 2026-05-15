using System.Globalization;

namespace Amigo.API.MiddleWare
{
    public class CultureMiddleware
    {
        private readonly RequestDelegate _next;

        public CultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var header = context.Request.Headers["Accept-Language"].ToString();

            var cultureName = header.Split(',')[0]; // en-US

            cultureName = cultureName.Split('-')[0]; // en

            var supported = new[] { "en", "fr", "es", "it", "pt", "pt-BR"};

            if (!supported.Contains(cultureName))
                cultureName = "en";

            var culture = new CultureInfo(cultureName);

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            await _next(context);
        }
    }
}

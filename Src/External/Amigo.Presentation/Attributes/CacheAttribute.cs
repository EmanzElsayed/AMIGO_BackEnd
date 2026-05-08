using Amigo.Application.Abstraction.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Attributes
{
    public class CacheAttribute(int duration = 100) : ActionFilterAttribute  // = Attribure + IAction Filter
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cachKey = CreateCacheKey(context.HttpContext.Request);

            var _cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var cacheValue = await _cacheService.GetAsync(cachKey);

            if (cacheValue is not null)
            {
                context.Result = new ContentResult()
                {
                    Content = cacheValue,
                    StatusCode = StatusCodes.Status200OK,
                    ContentType = "application/json",
                };
                return;
            }
            var executedContext = await next.Invoke();
            if (executedContext.Result is OkObjectResult result)
                await _cacheService.SetAsync(cachKey, result.Value, TimeSpan.FromSeconds(duration));
        }
        private string CreateCacheKey(HttpRequest request)
        {
            //baseurl/products?brandId=1&typeId=2
            StringBuilder Key = new StringBuilder();
            Key.Append(request.Path + "?");
            var QueryOrdered = request.Query.OrderBy(q => q.Key);
            foreach (var item in QueryOrdered)
            {
                Key.Append(item.Key + "=" + item.Value + "&");
            }

            return Key.ToString();
        }
    }
}

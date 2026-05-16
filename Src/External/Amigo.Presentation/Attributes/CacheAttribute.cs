using Amigo.Application.Abstraction.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

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
                context.Result = new JsonResult(
                  JsonSerializer.Deserialize<object>(cacheValue))
                {
                    StatusCode = StatusCodes.Status200OK
                };

                return;
            }
            var executedContext = await next.Invoke();
            if (executedContext.Result is ObjectResult result)
            {
                object valueToCache = result.Value!;

                var type = valueToCache.GetType();

                if (type.IsGenericType &&
                    type.GetProperty("Value") is not null)
                {


                    var isSuccess =
                    (bool)type.GetProperty("IsSuccess")!
                        .GetValue(valueToCache)!;

                    if (isSuccess)
                    {
                        valueToCache = type.GetProperty("Value")!
                            .GetValue(valueToCache)!;
                    }
                    else
                    {
                        return;
                    }
                }

                await _cacheService.SetAsync(
                    cachKey,
                    valueToCache,
                    TimeSpan.FromSeconds(duration));
            }
        }
        private string CreateCacheKey(HttpRequest request)
        {
            //baseurl/products?brandId=1&typeId=2
            StringBuilder Key = new StringBuilder();
            Key.Append(request.Path + "?");

            var language = request.Headers["Accept-Language"]
               .FirstOrDefault()?
               .Split(',')[0]
               .Split('-')[0]
               .ToLower();

            Key.Append($":lang-{language}");

            var QueryOrdered = request.Query.OrderBy(q => q.Key);
            foreach (var item in QueryOrdered)
            {
                Key.Append(item.Key + "=" + item.Value + "&");
            }

            return Key.ToString();
        }
    }
}

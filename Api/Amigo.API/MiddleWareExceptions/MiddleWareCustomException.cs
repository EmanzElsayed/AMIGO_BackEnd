using Amigo.Domain.Exceptions;
using Amigo.Domain.Exceptions.AlreadyExistExceptions;
using Amigo.SharedKernal.DTOs;

namespace Amigo.API.MiddleWareExceptions
{
    public class MiddleWareCustomException
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MiddleWareCustomException> _logger;

        public MiddleWareCustomException(RequestDelegate requestDelegate,
            ILogger<MiddleWareCustomException> logger)
        {
            _next = requestDelegate;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                // Handle 404 endpoint not found
                if (context.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var response = new ResultDTO<object>(
                        Data: null,
                        StatusCode: StatusCodes.Status404NotFound,
                        isSuccess: false,
                        ErrorMessages: new Dictionary<string, string>
                        {
                            { "Error", $"End Point {context.Request.Path} Is Not Found" }
                        }
                    );

                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong");

                int statusCode = ex switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    AlreadyExistException => StatusCodes.Status409Conflict,
                    BadRequestException => StatusCodes.Status400BadRequest,
                    UnUthorizedException => StatusCodes.Status401Unauthorized,
     
                    _ => StatusCodes.Status500InternalServerError
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                Dictionary<string, string>? errors = null;

                if (ex is BadRequestException badRequestException)
                {
                    errors = badRequestException.Errors;
                }
                else
                {
                    errors = new Dictionary<string, string>
                    {
                        { "Error", ex.Message }
                    };
                }

                var response = new ResultDTO<object>(
                    Data: null,
                    StatusCode: statusCode,
                    isSuccess: false,
                    ErrorMessages: errors,
                    Exception: ex
                );

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}

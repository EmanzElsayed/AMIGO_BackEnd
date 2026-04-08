using Amigo.Domain.Enum;
using Amigo.SharedKernal.DTOs.Results;
using System.Net;


namespace Amigo.API



{
   
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    #region Response Body
                    var Body = new ApiResponse<string>()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = $"End Point {context.Request.Path} Is Not Found",
                        ErrorCode =  ErrorCode.InvalidEndPoint.ToString()
                    };
                    await context.Response.WriteAsJsonAsync(Body);

                    #endregion
                }
                //if (context.Response.StatusCode == StatusCodes.Status400BadRequest)
                //{
                //    #region Response Body
                    
                //    var Body = new ApiResponse<string>()
                //    {
                //        StatusCode = context.Response.StatusCode,
                      
                //        Message = $"Body {context.Request.Body} Is Null",
                //        ErrorCode = ErrorCode.Forbidden.ToString()

                //    };
                //    await context.Response.WriteAsJsonAsync(Body);

                //    #endregion
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                var response = new ApiResponse<string>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    
                    ErrorCode = ErrorCode.InternalServerError.ToString(),
                    
                    TraceId = context.TraceIdentifier
                };


                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = response.StatusCode;
                    context.Response.ContentType = "application/json";
                     await context.Response.WriteAsJsonAsync(response);
                }

            }
        }
    }
}

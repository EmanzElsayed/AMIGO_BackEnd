using Amigo.Domain.Errors;
using Amigo.Domain.Errors.BusinessErrors;

using Amigo.SharedKernal.DTOs.Results;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Amigo.Presentation.Filters
{
    public class ResultFilter : IAsyncResultFilter
    {
        private int ResolveStatusCode(IResultBase result)
        {
            // Success case
            if (result.IsSuccess)
            {
                var successWithStatus = result.Successes.FirstOrDefault(s =>
                    s.Metadata.ContainsKey("StatusCode"));

                if (successWithStatus != null)
                    return Convert.ToInt32(successWithStatus.Metadata["StatusCode"]);

                return (int)HttpStatusCode.OK;
            }

            // Failure case
            return MapStatusCode(result.Errors);
        }
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult &&
                objectResult.Value is IResultBase result)
            {
                var response = Map(result, context);

                context.Result = new ObjectResult(response)
                {
                    StatusCode = ResolveStatusCode(result)
                };
            }

            await next();
        }

        private object Map(IResultBase result, ResultExecutingContext context)
        {
            object? value = null;

            // Check if result is generic Result<T>
            var resultType = result.GetType();
            if (result.IsSuccess &&
                resultType.IsGenericType &&
                resultType.GetGenericTypeDefinition() == typeof(FluentResults.Result<>))
            {
                value = resultType.GetProperty("Value")?.GetValue(result);
            }

            if (result.IsSuccess)
            {
                int statusCode = (int)HttpStatusCode.OK;

                var successWithStatus = result.Successes.FirstOrDefault(s =>
                    s.Metadata.ContainsKey("StatusCode")
                );
                if (successWithStatus != null)
                {
                    statusCode = Convert.ToInt32(successWithStatus.Metadata["StatusCode"]);
                }
                // 👇 Detect PaginatedResult
                if (value != null && IsPaginated(value))
                {
                    return MapPaginated(value, result, context, statusCode);
                }
                return new ApiResponse<object>
                {
                    IsSuccess = true,
                    StatusCode = statusCode,
                    Message = result.Successes.FirstOrDefault()?.Message,
                    Data = value, // safely assign Value only if exists
                    TraceId = context.HttpContext.TraceIdentifier
                };
            }

            return new ApiResponse<object>
            {
                IsSuccess = false,
                StatusCode = MapStatusCode(result.Errors),
                Message = string.Join(", ", result.Errors.Select(e => e.Message)),
                Errors = MapValidationErrors(result.Errors),
                ErrorCode = result.Errors.OfType<BaseDomainError>().FirstOrDefault()?.Code.ToString(),
                TraceId = context.HttpContext.TraceIdentifier
            };
        }
        private bool IsPaginated(object value)
        {
            var type = value.GetType();

            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(PaginatedResponse<>);
        }

        private object MapPaginated(object value, IResultBase result, ResultExecutingContext context, int statusCode)
        {
            var type = value.GetType();

            var data = type.GetProperty("Data")?.GetValue(value);
            var pageNumber = (int)(type.GetProperty("PageNumber")?.GetValue(value) ?? 1);
            var pageSize = (int)(type.GetProperty("PageSize")?.GetValue(value) ?? 10);
            var totalItems = (int)(type.GetProperty("TotalItems")?.GetValue(value) ?? 0);

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

           
            var responseType = typeof(PaginatedResponse<>)
                .MakeGenericType(type.GetGenericArguments()[0]);

            var response = Activator.CreateInstance(responseType);

            responseType.GetProperty("IsSuccess")?.SetValue(response, true);
            responseType.GetProperty("StatusCode")?.SetValue(response, statusCode);
            responseType.GetProperty("Message")?.SetValue(response, result.Successes.FirstOrDefault()?.Message);
            responseType.GetProperty("Data")?.SetValue(response, data);

            responseType.GetProperty("PageNumber")?.SetValue(response, pageNumber);
            responseType.GetProperty("PageSize")?.SetValue(response, pageSize);
            responseType.GetProperty("TotalItems")?.SetValue(response, totalItems);
            responseType.GetProperty("TotalPages")?.SetValue(response, totalPages);

            responseType.GetProperty("TraceId")?.SetValue(response, context.HttpContext.TraceIdentifier);

            return response!;
        }

        private int MapStatusCode(IReadOnlyList <IError> errors)
        {
            if (errors.OfType<ValidationErrror>().Any())
                return (int)HttpStatusCode.BadRequest;

            if (errors.OfType<EmailAlreadyExistsError>().Any())
                return (int)HttpStatusCode.Conflict;

            if (errors.OfType<ExceptionError>().Any())
                return (int)HttpStatusCode.InternalServerError;

            if(errors.OfType<NotFoundEmailError>().Any())
                return (int)HttpStatusCode.NotFound;

            if (errors.OfType<UnauthorizedError>().Any())
                return (int)HttpStatusCode.Unauthorized;

            if (errors.OfType<ForbiddenError>().Any())
                return (int)HttpStatusCode.Forbidden;
            
            return (int)HttpStatusCode.BadRequest;
        }

        private IEnumerable<ApiValidationError>? MapValidationErrors(IReadOnlyList<IError> errors)
        {
            return errors
                .OfType<ValidationErrror>()
                .SelectMany(e => e.Errors)
                .Select(e => new ApiValidationError
                {
                    Property = e.Property,
                    Messages = e.Messages
                });
        }

    }
}

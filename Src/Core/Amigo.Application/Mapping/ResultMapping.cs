using Amigo.Domain.Errors;
using Amigo.SharedKernal.DTOs;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public static class ResultMapping
    {
        /// <summary>
        /// Maps a FluentResults.Result<T> into ResultDTO<T> for API responses
        /// </summary>
        public static ResultDTO<T> ToResultDTO<T>(this Result<T> result)
        {
            
            if (result.IsSuccess)
            {
                return new ResultDTO<T>(
                    Data: result.Value, 
                    IsSuccess: true,
                    
                    Message: string.Join(", ", result.Successes.Select(s => s.Message)),
                    StatusCode : result.Successes
                                        .Select(s => s.Metadata.ContainsKey("StatusCode")
                                            ? (int)s.Metadata["StatusCode"]
                                            : 200)
                                        .FirstOrDefault()
                  );
            }

            // Extract Validation Errors if any
            var validationErrors = result.Errors
                .OfType<ValidationErrror>()
                .SelectMany(e => e.Error)
                .ToList();
            
            // Extract Domain/Business Errors
            var businessErrorMessage = result.Errors
                .OfType<BaseDomainError>()
                .FirstOrDefault()?.Message;

            return new ResultDTO<T>(

                Data: default,
               StatusCode:  result.Errors
                .Select(e => e.Metadata.ContainsKey("StatusCode")
                    ? (int)e.Metadata["StatusCode"]
                    : 400)
                .FirstOrDefault(),
               
                IsSuccess: false,
                Message: string.Join(", ", result.Errors.Select(e => e.Message)),
                Errors: validationErrors.Any() ? validationErrors : null
            );
        }
    }

}


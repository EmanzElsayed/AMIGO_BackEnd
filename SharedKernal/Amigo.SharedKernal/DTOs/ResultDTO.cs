namespace Amigo.SharedKernal.DTOs;

public record ResultDTO<T>
(
    T? Data,
    int StatusCode,
    bool IsSuccess = true,
    int Count = 0,
    int PageNumber = 1,
    int PageSize = 10,
    int TotalPages = 1,
    string Message = "Succeed",
    object? Errors = null,
    [property: JsonConverter(typeof(ExceptionConverter))] Exception? Exception = null
    
);

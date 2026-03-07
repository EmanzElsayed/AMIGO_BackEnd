namespace Amigo.SharedKernal.DTOs;

public record ResultDTO<T>
(
    T Data,
    int count = 0,
    int PageNumber = 1,
    int PageSize = 10,
    int TotalPages = 1,
    bool isSuccess = false,
    Dictionary<string, string>? Messages = null,
    Dictionary<string, string>? ErrorMessages = null,
    [property: JsonConverter(typeof(ExceptionConverter))] Exception? Exception = null
);

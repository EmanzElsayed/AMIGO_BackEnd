using FluentResults;

namespace Amigo.Domain.Errors;

public abstract class BaseDomainError(string msg, ErrorCode errorCode, int statusCode)
    : Error(msg)
{
    protected ErrorCode ErrorCode { get; set; } = errorCode;
    protected int StatusCode { get; set; } = statusCode;
    protected string Message { get; set; } = msg;

}

using FluentResults;

namespace Amigo.Domain.Errors;

public abstract class BaseDomainError
    : Error
{
    public ErrorCode Code { get;}
    protected BaseDomainError(string message, ErrorCode code)
         : base(message)
    {
        Code = code;
    }

}

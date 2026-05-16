using FluentResults;

namespace Amigo.Domain.Errors;

public abstract class BaseDomainError
    : Error
{
    public string Code { get;}
    public object[] Arguments { get; }
    protected BaseDomainError( string code, params object[] arguments)
        
    {
        Code = code;
        Arguments = arguments;
    }

}

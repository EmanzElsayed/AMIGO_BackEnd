namespace Amigo.Domain.Errors;

public class ValidationErrror(List<ValidationPropertError> errors,
                              string msg = "Invalid Body Input",
                              ErrorCode errorCode = ErrorCode.InvalidBodyInput,
                              int statusCode = 400) 
                              : BaseDomainError(msg, errorCode, statusCode)
{
    public List<ValidationPropertError> Error { get; set; } = errors;
}

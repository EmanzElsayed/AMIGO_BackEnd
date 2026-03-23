namespace Amigo.Domain.Errors;

public class ValidationErrror(List<ValidationPropertError> errors) : BaseDomainError("Invalid Body Input", ErrorCode.InvalidBodyInput, 400)
{
    public List<ValidationPropertError> Error { get; set; } = errors;
}

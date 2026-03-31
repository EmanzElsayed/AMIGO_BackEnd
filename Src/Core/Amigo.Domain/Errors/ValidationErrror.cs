namespace Amigo.Domain.Errors;

public class ValidationErrror
                              : BaseDomainError
{
    public List<ValidationPropertError> Errors { get; set; }

    public ValidationErrror(List<ValidationPropertError> errors,
        string msg = "Validation Failed")
        : base(msg, ErrorCode.InvalidBodyInput)
    {
        Errors = errors;
    }
}

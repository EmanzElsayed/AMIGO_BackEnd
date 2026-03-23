namespace Amigo.Domain.Errors;

public record ValidationPropertError(string Property,List<string> Messages)
{
    public ValidationPropertError(ValidationFailure validationFailure) : this(validationFailure.PropertyName, [validationFailure.ErrorMessage])
    {
        
    }
}

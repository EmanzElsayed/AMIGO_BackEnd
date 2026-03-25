namespace Amigo.Domain.Errors;

public record ValidationPropertError(string Property,List<string> Messages)
{

    // Constructor from ValidationFailure
    public ValidationPropertError(ValidationFailure failure)
        : this(failure.PropertyName, new List<string> { failure.ErrorMessage }) { }


        
    // Constructor from IdentityError
    public ValidationPropertError(string property, IEnumerable<string> messages)
        : this(property, messages.ToList()) { }
}

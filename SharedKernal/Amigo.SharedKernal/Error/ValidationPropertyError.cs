using System;
using System.Collections.Generic;
using System.Text;

using FluentValidation.Results;

namespace Amigo.SharedKernal.Error;
public record ValidationPropertyError(string Property, List<string> Messages)
{
    public ValidationPropertyError(ValidationFailure validationFailure) : this(validationFailure.PropertyName, [validationFailure.ErrorMessage])
    {

    }
}
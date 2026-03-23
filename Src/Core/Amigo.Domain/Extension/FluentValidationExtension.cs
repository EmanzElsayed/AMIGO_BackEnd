namespace Amigo.Domain.Extension;

public static class FluentValidationExtension
{
    public static async Task<Result> ValidateAndGroupErrorsAsync<Tin>(this IValidator<Tin> validator, Tin model)
    {
        if (validator is null) throw new ArgumentNullException(nameof(validator));
        if (model is null) throw new ArgumentNullException(nameof(model));

        var validationResult = await validator.ValidateAsync(model);

        if (validationResult.IsValid)
            return Result.Ok();

        var errors = validationResult
            .Errors
            .GroupBy(x => x.PropertyName)
            .Select(x => new ValidationPropertError(x.Key, x.Select(y => y.ErrorMessage).ToList()))
            .ToList();

        return Result.Fail(new ValidationErrror(errors));
    }
}

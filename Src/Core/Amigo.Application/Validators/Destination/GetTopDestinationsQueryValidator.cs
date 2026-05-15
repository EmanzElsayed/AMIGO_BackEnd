using Amigo.Application.Validation.Common.Rules;
using Amigo.SharedKernal.QueryParams;

namespace Amigo.Application.Validators.Destination;

public class GetTopDestinationsQueryValidator : AbstractValidator<GetTopDestinationsQuery>
{
    public GetTopDestinationsQueryValidator()
    {
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50);

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        
    }
}

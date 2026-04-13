using Amigo.Application.Specifications;
using Amigo.Application.Validation.Common.Specifications;
using Amigo.Domain.Entities;

namespace Amigo.Application.Specifications.DestinationSpecification.User;


public class TopDestinationsRankingSpecification : BaseSpecification<Destination, Guid>
{
    public TopDestinationsRankingSpecification()
        : base(TopDestinationCommonSpecification.BuildRankingEligibleCriteria())
    {
    }
}

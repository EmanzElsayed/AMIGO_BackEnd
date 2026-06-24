using Amigo.Domain.DTO.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction.Repositories
{
    public interface ITourRepo
    {

        Task<Dictionary<Guid, string?>> GetFirstTourImagesAsync(
         IEnumerable<Guid> tourIds);
        Task<List<Guid>> GetTourIdsWithDestinationId(Guid destinationId, UserType userType);
        Task<List<Guid>> GetTourIdsWithCountryId(Guid countryId, UserType userType);
        Task<int> GetDestinationCountWithCountryId(Guid countryId);
        Task<List<SearchResponseDTO>> SearchQueryInDestination(string query, SupportedLanguage language);
        Task<List<SearchResponseDTO>> SearchQueryInCountry(string query, SupportedLanguage language);

    }
}

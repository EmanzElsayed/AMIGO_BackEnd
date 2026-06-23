using Amigo.Domain.Abstraction.Repositories;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.Repositories
{
    public class TourRepo(AmigoDbContext _dbContext) 
        : ITourRepo
    {
        public async Task<Dictionary<Guid, string?>> GetFirstTourImagesAsync(
          IEnumerable<Guid> tourIds)
        {
            return await _dbContext.Tours
                .Where(t => tourIds.Contains(t.Id))
                .Select(t => new
                {
                    t.Id,
                    ImageUrl = t.Images
                        .OrderBy(i => i.CreatedDate)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
                })
                .ToDictionaryAsync(
                    x => x.Id,
                    x => x.ImageUrl);
        }

        public async Task<List<Guid>> GetTourIdsWithCountryId(Guid countryId, UserType userType)
        {
            return await  _dbContext.Tours
                                   .Where(t => t.Destination.CountryInfoId == countryId && ((t.UserType & userType) == userType))
                                   .Select(t => t.Id).ToListAsync();
        }

        public async Task<List<Guid>> GetTourIdsWithDestinationId(Guid destinationId,UserType userType)
        {

            return await _dbContext.Tours
                                   .Where(t => t.DestinationId == destinationId && ((t.UserType & userType) == userType))
                                   .Select(t => t.Id).ToListAsync();
            
        }
        public async Task<int> GetDestinationCountWithCountryId(Guid countryId)
        {

            return await _dbContext.Destinations
                                   .Where(t => t.CountryInfoId == countryId )
                                   .CountAsync();

        }
    }
}

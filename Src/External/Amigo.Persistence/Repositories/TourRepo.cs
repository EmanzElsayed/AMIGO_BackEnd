using Amigo.Domain.Abstraction.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.Repositories
{
    public class TourRepo(AmigoDbContext _dbContext) : ITourRepo
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

    }
}

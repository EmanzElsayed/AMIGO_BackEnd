using Amigo.Domain.Abstraction.Repositories;
using Amigo.Domain.DTO.Favorite;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amigo.Persistence.Repositories;

public class FavoriteRepo(AmigoDbContext _dbContext) : IFavoriteRepo
{
    public async Task<List<FavoriteResponseDTO>> GetUserFavoritesAsync(string userId)
    {
        var rows = await _dbContext.Favorites.AsNoTracking()
            .Where(f => f.UserId == userId && !f.IsDeleted)
            .Join(_dbContext.Tours.AsNoTracking().Where(t => !t.IsDeleted),
                f => f.TourId,
                t => t.Id,
                (f, t) => new { f, t })
            .Select(x => new FavoriteResponseDTO
            {
                TourId = x.t.Id,
                Title = x.t.Translations
                    .Where(tr => !tr.IsDeleted)
                    .OrderBy(tr => tr.Language == SupportedLanguage.en ? 0 : 1)
                    .Select(tr => tr.Title)
                    .FirstOrDefault() ?? "Tour",
                DestinationId = x.t.DestinationId,
                CoverImageUrl = _dbContext.TourImages
                    .Where(img => img.TourId == x.t.Id && !img.IsDeleted)
                    .Select(img => img.ImageUrl)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return rows;
    }

    public async Task<bool> ExistsAsync(string userId, Guid tourId)
    {
        return await _dbContext.Favorites.AnyAsync(f =>
            f.UserId == userId && f.TourId == tourId && !f.IsDeleted);
    }

    public async Task AddAsync(Favorites favorite)
    {
        await _dbContext.Favorites.AddAsync(favorite);
    }

    public async Task<Favorites?> GetByUserAndTourAsync(string userId, Guid tourId)
    {
        return await _dbContext.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.TourId == tourId && !f.IsDeleted);
    }

    public void Update(Favorites favorite)
    {
        _dbContext.Favorites.Update(favorite);
    }
}

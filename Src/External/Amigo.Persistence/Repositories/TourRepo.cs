using Amigo.Application.Helpers;
using Amigo.Domain.Abstraction.Repositories;
using Amigo.Domain.DTO.Search;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Stripe;
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

        public async Task<List<SearchResponseDTO>> SearchQueryInDestination(string query, SupportedLanguage language)
        {
            var result = await _dbContext.Destinations
            .Where(d => !d.IsDeleted &&
            d.Translations.Any(tr =>
            tr.Language == language &&
            tr.Name.ToLower().Trim().StartsWith(query.ToLower().Trim())))
            .Select(d => new
            {
                
                DestinationName = d.Translations
                    .Where(tr => tr.Language == language)
                    .Select(tr => tr.Name)
                    .FirstOrDefault(),

                CountryName = d.CountryInfo.Translations
                    .Where(tr => tr.Language == language)
                    .Select(tr => tr.Name)
                    .FirstOrDefault()
            })
            .ToListAsync();
            return result.Select(r => new SearchResponseDTO(
                    DestinationName : r.DestinationName,
                    DestinationSlug : SlugHelper.ToUrlSlug(r.DestinationName),
                    CountryName : r.CountryName,
                    CountrySlug : SlugHelper.ToUrlSlug(r.CountryName)
                
                )).ToList();
        }
        public async Task<List<SearchResponseDTO>> SearchQueryInCountry(string query, SupportedLanguage language)
        {
            var result = await _dbContext.CountryInfo
            .Where(d => !d.IsDeleted &&
            d.Translations.Any(tr =>
            tr.Language == language &&
            tr.Name.ToLower().Trim().StartsWith(query.ToLower().Trim())))
            .Select(d => new
            {

                CountryName = d.Translations
                    .Where(tr => tr.Language == language)
                    .Select(tr => tr.Name)
                    .FirstOrDefault(),
               
            })
            .ToListAsync();
            return result.Select(r => new SearchResponseDTO(
                    DestinationName: "",
                    DestinationSlug: "",
                    CountryName: r.CountryName,
                    CountrySlug: SlugHelper.ToUrlSlug(r.CountryName)

                )).ToList();
        }
    }
}

using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Translation;
using Amigo.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.Services
{
    public class TourTranslationQueryService :ITourTranslationQueryService
    {
        private readonly AmigoDbContext _context;

        public TourTranslationQueryService(AmigoDbContext context)
        {
            _context = context;
        }

        public async Task<List<TourTranslationItem>>
            GetPendingTranslationToursAsync(SupportedLanguage baseLanguage)
        {
            return await _context.Tours
                .AsNoTracking()
                .Select(t => new TourTranslationItem
                {
                    TourId = t.Id,
                    SourceLanguage = baseLanguage,

                    Title = t.Translations
                        .Where(x => x.Language == baseLanguage)
                        .Select(x => x.Title)
                        .FirstOrDefault() ?? "",

                    Description = t.Translations
                        .Where(x => x.Language == baseLanguage)
                        .Select(x => x.Description)
                        .FirstOrDefault(),

                    Destination = new DestinationTranslationItem
                    {
                        DestinationId = t.DestinationId,
                        Name = t.Destination.Translations
                            .Where(x => x.Language == baseLanguage)
                            .Select(x => x.Name)
                            .FirstOrDefault() ?? ""
                    },

                    Cancellation = new CancellationTranslationItem
                    {
                        CancellationId = t.Cancellation.Id,
                        Description = t.Cancellation.Translations
                            .Where(x => x.Language == baseLanguage)
                            .Select(x => x.Description)
                            .FirstOrDefault() ?? ""
                    },

                    Inclusions = t.TourInclusions
                        .Select(i => new InclusionTranslationItem
                        {
                            InclusionId = i.Id,
                            Text = i.Translations
                                .Where(x => x.Language == baseLanguage)
                                .Select(x => x.Text)
                                .FirstOrDefault() ?? ""
                        })
                        .ToList(),

                    Prices = t.Prices
                        .Select(p => new PriceTranslationItem
                        {
                            PriceId = p.Id,
                            Type = p.Translations
                                .Where(x => x.Language == baseLanguage)
                                .Select(x => x.Type)
                                .FirstOrDefault() ?? ""
                        })
                        .ToList()
                })
                .ToListAsync();
        }

       
    }
}

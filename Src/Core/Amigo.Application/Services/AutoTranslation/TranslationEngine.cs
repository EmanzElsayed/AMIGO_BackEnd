using Amigo.Application.Specifications.TourSpecification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.AutoTranslation
{
    public class TranslationEngine(IUnitOfWork _unitOfWork, TranslationService _api)
    {
        //private readonly TranslationService _api;

        //public TranslationEngine(TranslationService api)
        //{
        //    _api = api;
        //}

        public async Task TranslateAsync<TTranslation>(
            ICollection<TTranslation> translations,
            Func<TTranslation> createTranslation,
            Dictionary<string, Func<TTranslation, string>> getters,
            Dictionary<string, Action<TTranslation, string>> setters,
            Language sourceLang)
            where TTranslation : ITranslationEntity
        {
            var languages = Enum.GetValues<Language>()
                    .Where(l => l != Language.None && l != sourceLang)
                    .ToList();

            var fromCode = LanguageMapper.ToCode(sourceLang);

            var tasks = languages.Select(async lang =>
            {
                if (translations.Any(t => t.Language == lang))
                    return;

                var toCode = LanguageMapper.ToCode(lang);
                var fromCode = LanguageMapper.ToCode(sourceLang);

                var sample = translations.FirstOrDefault(t => t.Language == sourceLang);

                if (sample == null)
                    throw new Exception("Source translation missing");

                var texts = getters.Values
                    .Select(g => g(sample))
                    .Where(v => !string.IsNullOrEmpty(v))
                    .ToList();

                var translated = await _api.TranslateBatchAsync(texts, fromCode, toCode);

                var newTranslation = createTranslation();
                newTranslation.Language = lang;

                int i = 0;
                foreach (var key in getters.Keys)
                {
                    var value = getters[key](sample);
                    if (!string.IsNullOrEmpty(value))
                    {
                        setters[key](newTranslation, translated[i++]);
                    }
                }

                lock (translations) // مهم جدًا عشان Thread safety
                {
                    translations.Add(newTranslation);
                }
            });

            await Task.WhenAll(tasks);
        }


        public async Task TranslateDestination(Guid destinationId, Language sourceLang)
        {
            var _repo = _unitOfWork.GetRepository<Destination, Guid>();
            var destination = await _repo.GetByIdAsync(new GetDestinationWithTranslationsSpecification(destinationId));

            var getters = new Dictionary<string, Func<DestinationTranslation, string>>
            {
                { "Name", x => x.Name }
            };

            var setters = new Dictionary<string, Action<DestinationTranslation, string>>
            {
                { "Name", (x, value) => x.Name = value }
            };

            await TranslateAsync(
                destination.Translations,
                () => new DestinationTranslation(),
                getters,
                setters,
                sourceLang
            );

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Result> TranslateTour(Guid tourId, Language sourceLang)
        {
            var repo = _unitOfWork.GetRepository<Tour, Guid>();

            var tour = await repo.GetByIdAsync(new GetTourByIdSpecification(tourId));

            if (tour == null)
                throw new Exception("Tour not found");

            var strategy = _unitOfWork.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // ======================
                    // Tour Translation
                    // ======================
                    await TranslateAsync(
                        tour.Translations,
                        () => new TourTranslation(),
                        new Dictionary<string, Func<TourTranslation, string>>
                        {
                { "Title", x => x.Title },
                { "Description", x => x.Description }
                        },
                        new Dictionary<string, Action<TourTranslation, string>>
                        {
                { "Title", (x, v) => x.Title = v },
                { "Description", (x, v) => x.Description = v }
                        },
                        sourceLang
                    );

                    // ======================
                    // Prices
                    // ======================
                    foreach (var price in tour.Prices)
                    {
                        await TranslateAsync(
                            price.Translations,
                            () => new PriceTranslation(),
                            new Dictionary<string, Func<PriceTranslation, string>>
                            {
                    { "Type", x => x.Type }
                            },
                            new Dictionary<string, Action<PriceTranslation, string>>
                            {
                    { "Type", (x, v) => x.Type = v }
                            },
                            sourceLang
                        );
                    }

                    // ======================
                    // Cancellation
                    // ======================
                    if (tour.Cancellation != null)
                    {
                        await TranslateAsync(
                            tour.Cancellation.Translations,
                            () => new CancellationTranslation(),
                            new Dictionary<string, Func<CancellationTranslation, string>>
                            {
                    { "Description", x => x.Description }
                            },
                            new Dictionary<string, Action<CancellationTranslation, string>>
                            {
                    { "Description", (x, v) => x.Description = v }
                            },
                            sourceLang
                        );
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Result.Ok(); // ✅ مهم جدًا
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return FluentValidationExtension.FromException(details: ex.Message);
                }
            });




        }
}   }

using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.Translation
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
 }  }

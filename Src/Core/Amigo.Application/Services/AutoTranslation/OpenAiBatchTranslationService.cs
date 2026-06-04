using Amigo.Application.Helpers;
using Amigo.Domain.DTO.Translation;
using OpenAI.Chat;
using System.Text.Json;

namespace Amigo.Application.Services.AutoTranslation
{
   
    public class OpenAiBatchTranslationService
        : IOpenAiBatchTranslationService
    {
        private readonly ChatClient _client;

        public OpenAiBatchTranslationService(
            IConfiguration configuration)
        {
            var apiKey =
                configuration["OpenAiSettings:ApiKey"];

            _client = new ChatClient(
                model: "gpt-4o-mini",
                apiKey: apiKey);
        }

        public Task<List<DestinationTranslationAiResult>> TranslateDestinationAsync(DestinationTranslationItem destination, SupportedLanguage sourceLanguage)
        {
            throw new NotImplementedException();
        }

        public Task<List<TourTranslationAiResult>> TranslateTourAsync(TourTranslationItem tour, SupportedLanguage sourceLanguage)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ToursTranslationAiResult>>
            TranslateToursAsync(
                List<TourTranslationItem> tours)
        {
            if (!tours.Any())
                return new();


            var sourceLanguage = tours.First().SourceLanguage;

            var targetLanguages =
                TranslationLanguageHelper
                    .GetTargetLanguages(sourceLanguage);


            var payload = new
            {
                

                targetLanguages = targetLanguages
                     .Select(x => x.ToString()),

                tours
            };

            var json = JsonSerializer.Serialize(
                payload,
                new JsonSerializerOptions
                {
                    WriteIndented = false
                });

            var schema =
                JsonSchemaHelper.GenerateToursTranslationSchema();

            var prompt = $"""
                You are a professional tourism translation engine.

                Translate ALL tourism content from.
                SOURCE LANGUAGE:
                {sourceLanguage}

                TARGET LANGUAGES (MUST ONLY RETURN THESE):
                {string.Join(", ", targetLanguages)}


                IMPORTANT RULES:
                - Return VALID JSON ONLY
                - No markdown
                - No explanations
                - Preserve IDs
                - Preserve JSON structure exactly
                - Preserve HTML tags if exist
                - Translate ALL text fields
                - Keep property names unchanged

                RESPONSE SCHEMA:
                {schema}

                INPUT:
                {json}
                """;

            try
            {
                var response = await _client.CompleteChatAsync(prompt);
                var content =
                 response.Value.Content[0].Text;

                var result = JsonSchemaHelper.DeserializeOrThrow<
                    List<ToursTranslationAiResult>
                >(content);

                return result ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
         
        }
    }
}

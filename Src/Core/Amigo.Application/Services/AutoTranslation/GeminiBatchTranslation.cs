using Amigo.Application.Helpers;
using Amigo.Domain.DTO.Translation;
using Amigo.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Amigo.Application.Services.AutoTranslation
{
    public class GeminiBatchTranslationService : IOpenAiBatchTranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _url;
        public GeminiBatchTranslationService(
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _apiKey = configuration["OpenAiSettings:GeminiKey"];
            _url = configuration["OpenAiSettings:GeminiUrl"];
            _httpClient = httpClient;
        }

        public async Task<List<DestinationTranslationAiResult>> TranslateDestinationAsync(DestinationTranslationItem destination, SupportedLanguage sourceLanguage)
        {
            if (destination is null)
                return new();

            var targetLanguages =
              TranslationLanguageHelper.GetTargetLanguages(sourceLanguage);

            var payload = new
            {
                targetLanguages = targetLanguages.Select(x => x.ToString()),
                destination
            };
            var json = JsonSerializer.Serialize(payload);
            var schema =
             JsonSchemaHelper.GenerateDestinationTranslationSchema();

            var prompt = $"""
                        You are a professional tourism translation engine.

                        Translate ALL Destination content.

                        SOURCE LANGUAGE:
                        {sourceLanguage}

                        TARGET LANGUAGES (MUST ONLY RETURN THESE):
                        {string.Join(", ", targetLanguages)}

                        IMPORTANT RULES:
                        You MUST return ALL fields in schema.

                        - When language = "br", "br" means Brazilian Portuguese
                        - Return VALID JSON ONLY
                        - No markdown
                        - No explanations
                        - Preserve IDs
                        - Preserve structure exactly
                        - Preserve HTML tags if exist
                        - Translate all text fields
                        - Keep property names unchanged
                        - DO NOT translate null values
                        - If a field value is null, return it as null
                        - Never replace null with text, empty string, or translated content
                        - Only translate fields that contain actual text
                        RESPONSE SCHEMA:
                        {schema}

                        INPUT:
                        {json}
                        """;

            try
            {
                // Gemini Request Body
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var requestJson = JsonSerializer.Serialize(requestBody);

                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"{_url}?key={_apiKey}"
                );

                request.Content = new StringContent(
                    requestJson,
                    Encoding.UTF8,
                    "application/json"
                );
                return await ExecuteWithRetryAsync(async () =>
              {
                  var response = await _httpClient.SendAsync(request);

                  var responseString = await response.Content.ReadAsStringAsync();

                  if (!response.IsSuccessStatusCode)
                  {
                      throw new Exception(
                          $"Gemini API Error: {response.StatusCode} - {responseString}"
                      );
                  }

                  //  Parse Gemini response
                  using var doc = JsonDocument.Parse(responseString);

                  var text = doc.RootElement
                      .GetProperty("candidates")[0]
                      .GetProperty("content")
                      .GetProperty("parts")[0]
                      .GetProperty("text")
                      .GetString();

                  if (string.IsNullOrWhiteSpace(text))
                      return new();

                  //  Deserialize your structured result
                  var result =
                      JsonSchemaHelper.DeserializeOrThrow<List<DestinationTranslationAiResult>>(text);

                  return result ?? new();

              });

            }
            catch (Exception ex)
            {
                Console.WriteLine("Gemini Translation Error:");
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<List<TourTranslationAiResult>> TranslateTourAsync(
            TourTranslationItem tour,
            SupportedLanguage sourceLanguage
            )
        {
            if (tour is null)
                return new();

            var targetLanguages =
               TranslationLanguageHelper.GetTargetLanguages(sourceLanguage);

            var payload = new
            {
                targetLanguages = targetLanguages.Select(x => x.ToString()),
                tour
            };
            var json = JsonSerializer.Serialize(payload);
            var schema =
             JsonSchemaHelper.GenerateTourTranslationSchema();

            var prompt = $"""
                        You are a professional tourism translation engine.

                        Translate ALL tourism content.

                        SOURCE LANGUAGE:
                        {sourceLanguage}

                        TARGET LANGUAGES (MUST ONLY RETURN THESE):
                        {string.Join(", ", targetLanguages)}

                        IMPORTANT RULES:
                        You MUST return ALL fields in schema.

                        - When language = "br", "br" means Brazilian Portuguese
                        - Return VALID JSON ONLY
                        - No markdown
                        - No explanations
                        - Preserve IDs
                        - Preserve structure exactly
                        - Preserve HTML tags if exist
                        - Translate all text fields
                        - Keep property names unchanged
                        - DO NOT translate null values
                        - If a field value is null, return it as null
                        - Never replace null with text, empty string, or translated content
                        - Only translate fields that contain actual text
                        RESPONSE SCHEMA:
                        {schema}

                        INPUT:
                        {json}
                        """;

            try
            {
                // Gemini Request Body
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var requestJson = JsonSerializer.Serialize(requestBody);

                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"{_url}?key={_apiKey}"
                );

                request.Content = new StringContent(
                    requestJson,
                    Encoding.UTF8,
                    "application/json"
                );
                return await ExecuteWithRetryAsync(async () =>
                {
                    var response = await _httpClient.SendAsync(request);

                    var responseString = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(
                            $"Gemini API Error: {response.StatusCode} - {responseString}"
                        );
                    }

                    //  Parse Gemini response
                    using var doc = JsonDocument.Parse(responseString);

                    var text = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    if (string.IsNullOrWhiteSpace(text))
                        return new();

                    //  Deserialize your structured result
                    var result =
                        JsonSchemaHelper.DeserializeOrThrow<List<TourTranslationAiResult>>(text);

                    return result ?? new();

                });
            }
                
            catch (Exception ex)
            {
                Console.WriteLine("Gemini Translation Error:");
                Console.WriteLine(ex.ToString());
                throw;
            }

        }

        public async Task<List<ToursTranslationAiResult>> TranslateToursAsync(
            List<TourTranslationItem> tours)
        {
            if (!tours.Any())
                return new();

            var sourceLanguage = tours.First().SourceLanguage;

            var targetLanguages =
                TranslationLanguageHelper.GetTargetLanguages(sourceLanguage);

            var payload = new
            {
                targetLanguages = targetLanguages.Select(x => x.ToString()),
                tours
            };

            var json = JsonSerializer.Serialize(payload);

            var schema =
                JsonSchemaHelper.GenerateToursTranslationSchema();

                       var prompt = $"""
                        You are a professional tourism translation engine.

                        Translate ALL tourism content.

                        SOURCE LANGUAGE:
                        {sourceLanguage}

                        TARGET LANGUAGES (MUST ONLY RETURN THESE):
                        {string.Join(", ", targetLanguages)}

                        IMPORTANT RULES:
                        You MUST return ALL fields in schema.

                        - When language = "br", "br" means Brazilian Portuguese
                        - Return VALID JSON ONLY
                        - No markdown
                        - No explanations
                        - Preserve IDs
                        - Preserve structure exactly
                        - Preserve HTML tags if exist
                        - Translate all text fields
                        - Keep property names unchanged

                        RESPONSE SCHEMA:
                        {schema}

                        INPUT:
                        {json}
                        """;

            try
            {
                // Gemini Request Body
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var requestJson = JsonSerializer.Serialize(requestBody);

                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"{_url}?key={_apiKey}"
                );

                request.Content = new StringContent(
                    requestJson,
                    Encoding.UTF8,
                    "application/json"
                );
                return await ExecuteWithRetryAsync(async () =>
                {
                    var response = await _httpClient.SendAsync(request);

                    var responseString = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(
                            $"Gemini API Error: {response.StatusCode} - {responseString}"
                        );
                    }

                    //  Parse Gemini response
                    using var doc = JsonDocument.Parse(responseString);

                    var text = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    if (string.IsNullOrWhiteSpace(text))
                        return new();

                    //  Deserialize your structured result
                    var result =
                        JsonSchemaHelper.DeserializeOrThrow<List<ToursTranslationAiResult>>(text);

                    return result ?? new();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Gemini Translation Error:");
                Console.WriteLine(ex.ToString());
                throw;
            }
        }


        private async Task<T> ExecuteWithRetryAsync<T>(
             Func<Task<T>> action)
        {
            const int maxRetries = 3;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await action();
                }
                catch
                {
                    if (attempt == maxRetries)
                        throw;

                    await Task.Delay(attempt * 2000);
                }
            }

            throw new InvalidOperationException();
        }
    }
}
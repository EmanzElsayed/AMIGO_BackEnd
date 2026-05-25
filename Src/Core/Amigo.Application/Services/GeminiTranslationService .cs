using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Amigo.Application.Services
{
    public class GeminiTranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GeminiTranslationService> _logger;

        public GeminiTranslationService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<GeminiTranslationService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Dictionary<string, string>> TranslateAsync(
            string text,
            IReadOnlyList<string> targetLanguageCodes,
            CancellationToken cancellationToken = default)
        {
            var languages = targetLanguageCodes.ToList();
            _logger.LogInformation("Starting translation request for text: '{Text}' to languages: {Languages}",
                text.Length > 100 ? text.Substring(0, 100) + "..." : text,
                string.Join(", ", languages));

            var apiKey =
                _configuration["GeminiSettings:ApiKey"];

            var languageList =
                string.Join(", ", languages);

            var prompt = $$"""
            You are a professional translator. Translate the following text EXACTLY and PRECISELY into the specified languages.

            IMPORTANT RULES:
            1. Return ONLY valid JSON object (no markdown, no code blocks, no extra text)
            2. Use EXACTLY these language codes as keys (case-sensitive): {{languageList}}
            3. Provide clean, natural translations without any extra characters or repetition
            4. Do NOT add any explanation, commentary, or additional text
            5. JSON format MUST be valid and parseable
            6. Keys must be lowercase language codes exactly as specified above

            Example output format:
            {"en": "translated text", "es": "texto traducido", "fr": "texte traduit"}

            Text to translate:
            {{text}}

            Return ONLY the JSON object with translations using the exact language codes as keys.
            """;

            _logger.LogDebug("Prompt being sent to Gemini: {Prompt}", prompt);

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                }
            };

            var json =
                JsonSerializer.Serialize(requestBody);

            const int maxRetries = 3;

            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    _logger.LogDebug("Attempt {Retry} of {MaxRetries} to call Gemini API", retry + 1, maxRetries);

                    var response = await _httpClient.PostAsync(
          $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={apiKey}",
                              new StringContent(
                                json,
                                Encoding.UTF8,
                                "application/json"),
                              cancellationToken);

                    if ((int)response.StatusCode == 429)
                    {
                        _logger.LogWarning("Rate limited (429). Waiting before retry {Retry}", retry + 1);
                        await Task.Delay((retry + 1) * 3000, cancellationToken);
                        continue;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("API call failed with status {StatusCode}: {ReasonPhrase}",
                            response.StatusCode, response.ReasonPhrase);
                        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                        _logger.LogError("Error response: {ErrorContent}", errorContent);
                        return [];
                    }

                    var responseContent =
                        await response.Content.ReadAsStringAsync(cancellationToken);

                    _logger.LogDebug("Raw API response: {Response}", responseContent);

                    using var document =
                        JsonDocument.Parse(responseContent);

                    var result =
                        document.RootElement
                                .GetProperty("candidates")[0]
                                .GetProperty("content")
                                .GetProperty("parts")[0]
                                .GetProperty("text")
                                .GetString();

                    if (string.IsNullOrWhiteSpace(result))
                    {
                        _logger.LogWarning("API returned empty result");
                        return [];
                    }

                    _logger.LogInformation("Raw translation result from Gemini: {Result}", result);

                    result = result
                        .Replace("```json", "")
                        .Replace("```", "")
                        .Trim();

                    _logger.LogInformation("Cleaned result (after removing markdown): {CleanedResult}", result);

                    if (!result.StartsWith("{") || !result.EndsWith("}"))
                    {
                        _logger.LogError("Invalid JSON format. Result should start with '{{' and end with '}}'. Got: {Result}", result);
                        return [];
                    }

                    var translations =
                        JsonSerializer.Deserialize<
                            Dictionary<string, string>>(result);

                    if (translations == null || translations.Count == 0)
                    {
                        _logger.LogWarning("Deserialized translations are null or empty");
                        return [];
                    }

                    _logger.LogInformation("Successfully deserialized {TranslationCount} translations", translations.Count);

                    var normalizedTranslations = translations
                        .ToDictionary(
                            x => x.Key.Trim().ToLowerInvariant(),
                            x => x.Value.Trim());

                    foreach (var kvp in normalizedTranslations)
                    {
                        _logger.LogInformation("Translation - Language: '{Language}', Text: '{TranslatedText}'",
                            kvp.Key, kvp.Value);
                    }

                    return normalizedTranslations;
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "JSON parsing error on attempt {Retry}", retry + 1);
                    if (retry == maxRetries - 1) throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error on attempt {Retry}", retry + 1);
                    if (retry == maxRetries - 1) throw;
                }
            }

            _logger.LogError("Translation failed after {MaxRetries} retries", maxRetries);
            return [];
        }

        public async Task<Dictionary<string, Dictionary<string, string>>> TranslateBatchAsync(
            IReadOnlyList<string> texts,
            IReadOnlyList<string> targetLanguageCodes,
            CancellationToken cancellationToken = default)
        {
            if (texts == null || texts.Count == 0) return new Dictionary<string, Dictionary<string, string>>();

            var languages = string.Join(", ", targetLanguageCodes);
            var itemsToTranslate = texts.Select((text, index) => new { id = index, text = text }).ToList();
            var jsonInput = JsonSerializer.Serialize(itemsToTranslate);

            var prompt = $$"""
            You are an expert, professional translator specializing in travel and tourism.
            I will provide a JSON array of objects, each containing an integer "id" and a "text" string.
            Translate the "text" EXACTLY and PRECISELY into the following languages: {{languages}}

            IMPORTANT RULES:
            1. Return ONLY a valid JSON array of objects (no markdown, no extra text, no code blocks).
            2. Each object in the array must correspond to the "id" from the input.
            3. Use EXACTLY these language codes as keys for the translations (case-sensitive lowercase): {{languages}}
            4. Provide clean, natural, and highly professional translations suitable for a premium tour catalog.
            5. Do NOT add any explanation, commentary, or conversational filler.
            6. The JSON format MUST be valid and directly parseable.

            Expected Output JSON format:
            [
              {
                "id": 0,
                "translations": { "en": "translated text...", "es": "texto traducido..." }
              }
            ]

            Input to translate:
            {{jsonInput}}
            """;

            _logger.LogInformation("Starting BATCH translation request for {Count} items to languages: {Languages}", texts.Count, languages);
            _logger.LogDebug("Batch Prompt: {Prompt}", prompt);

            var apiKey = _configuration["GeminiSettings:ApiKey"];
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

            var json = JsonSerializer.Serialize(requestBody);
            const int maxRetries = 3;

            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    _logger.LogDebug("Batch attempt {Retry} of {MaxRetries}", retry + 1, maxRetries);

                    var response = await _httpClient.PostAsync(
                        $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={apiKey}",
                        new StringContent(json, Encoding.UTF8, "application/json"),
                        cancellationToken);

                    if ((int)response.StatusCode == 429)
                    {
                        _logger.LogWarning("Rate limited (429) on batch translation. Waiting...");
                        await Task.Delay((retry + 1) * 3000, cancellationToken);
                        continue;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                        _logger.LogError("Batch API call failed with status {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);
                        return new Dictionary<string, Dictionary<string, string>>();
                    }

                    var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    using var document = JsonDocument.Parse(responseContent);

                    var rawResult = document.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    if (string.IsNullOrWhiteSpace(rawResult))
                    {
                        _logger.LogWarning("Batch API returned empty result");
                        return new Dictionary<string, Dictionary<string, string>>();
                    }

                    rawResult = rawResult.Replace("```json", "").Replace("```", "").Trim();

                    using var resultDoc = JsonDocument.Parse(rawResult);
                    if (resultDoc.RootElement.ValueKind != JsonValueKind.Array)
                    {
                        _logger.LogError("Expected JSON array from batch translation. Got: {Kind}", resultDoc.RootElement.ValueKind);
                        return new Dictionary<string, Dictionary<string, string>>();
                    }

                    var resultDict = new Dictionary<string, Dictionary<string, string>>();

                    foreach (var element in resultDoc.RootElement.EnumerateArray())
                    {
                        var id = element.GetProperty("id").GetInt32();
                        var transNode = element.GetProperty("translations");

                        var translationsDict = new Dictionary<string, string>();
                        foreach (var prop in transNode.EnumerateObject())
                        {
                            translationsDict[prop.Name.Trim().ToLowerInvariant()] = prop.Value.GetString()?.Trim() ?? "";
                        }

                        if (id >= 0 && id < texts.Count)
                        {
                            var originalText = texts[id];
                            resultDict[originalText] = translationsDict;
                        }
                    }

                    _logger.LogInformation("Successfully mapped {Count} batch translations.", resultDict.Count);
                    return resultDict;
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "JSON parsing error on batch attempt {Retry}", retry + 1);
                    if (retry == maxRetries - 1) throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error on batch attempt {Retry}", retry + 1);
                    if (retry == maxRetries - 1) throw;
                }
            }

            _logger.LogError("Batch translation failed after {MaxRetries} retries", maxRetries);
            return new Dictionary<string, Dictionary<string, string>>();
        }
    }
}
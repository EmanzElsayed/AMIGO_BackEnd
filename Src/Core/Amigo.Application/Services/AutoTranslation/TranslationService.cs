using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Amigo.Application.Services.AutoTranslation
{
    public class TranslationService
    {
        private readonly HttpClient _httpClient;

        public TranslationService(
            HttpClient httpClient
            )
        {
            _httpClient = httpClient;

            _httpClient.BaseAddress = new Uri("http://127.0.0.1:8000");

            _httpClient.DefaultRequestHeaders.Add("x-api-key", "w(_Ul*BRL]Jw.]xl*QIiX5jhMTXE:M.B&l");
        }

        public async Task<List<string>> TranslateBatchAsync(
            List<string> texts,
            string from,
            string to)
        {
            try
            {
                var request = new
                {
                    texts,
                    from_lang = from,
                    to_lang = to
                };

                var response = await _httpClient.PostAsJsonAsync("/translate-batch", request);

                // ❌ API returned error (500, 403, 404...)
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();

                    throw new Exception(
                        $"Translation API failed. Status: {(int)response.StatusCode}, Body: {errorBody}"
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<TranslationResponse>();

                if (result?.Results == null)
                    throw new Exception("Translation API returned empty response");

                return result.Results;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Translation API is unreachable (network error)", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new Exception("Translation API timeout", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error in TranslationService: {ex.Message}", ex);
            }
        }
    }

    public class TranslationResponse
    {
        public List<string> Results { get; set; }
    }
}
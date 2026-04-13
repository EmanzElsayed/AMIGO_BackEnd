using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Amigo.Application.Services.Translation
{
    public class TranslationService
    {
        private readonly HttpClient _httpClient;

        public TranslationService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _httpClient.BaseAddress = new Uri("http://127.0.0.1:8000");

            _httpClient.DefaultRequestHeaders.Add("x-api-key", "w(_Ul*R]Jw.]xl*QIiX5jhMTXE:M.B&l");
        }

        public async Task<List<string>> TranslateBatchAsync(
            List<string> texts,
            string from,
            string to)
        {
            var request = new
            {
                texts = texts,
                from_lang = from,
                to_lang = to
            };

            var response = await _httpClient.PostAsJsonAsync("/translate-batch", request);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Translation API failed");

            var result = await response.Content.ReadFromJsonAsync<TranslationResponse>();

            return result.Results;
        }
    }

    public class TranslationResponse
    {
        public List<string> Results { get; set; }
    }
}


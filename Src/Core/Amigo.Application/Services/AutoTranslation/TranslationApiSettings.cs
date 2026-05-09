using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.AutoTranslation
{
    public class TranslationApiSettings
    {
        public required string BaseUrl { get; set; }
        public required string ApiKey { get; set; }
    }
}

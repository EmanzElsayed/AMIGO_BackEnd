using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Amigo.Application.Helpers
{
    public static class JsonSchemaHelper
    {
        public static string GenerateTourTranslationSchema()
        {
            return """
            [
              {
                "language": "string",
                "tours": [
                  {
                    "tourId": "guid",
                    "title": "string",
                    "description": "string",

                    "destinationTranslationItem": {
                      "destinationId": "guid",
                      "name": "string"
                    },

                    "cancellationTranslationItem": {
                      "cancellationId": "guid",
                      "description": "string"
                    },

                    "inclusionTranslationItem": [
                      {
                        "inclusionId": "guid",
                        "text": "string"
                      }
                    ],

                    "priceTranslationItem": [
                      {
                        "priceId": "guid",
                        "type": "string"
                      }
                    ]
                  }
                ]
              }
            ]
            """;
        }

        public static T DeserializeOrThrow<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })!;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Invalid AI JSON Response: {ex.Message}\n\nRAW:\n{json}");
            }
        }
    }
}

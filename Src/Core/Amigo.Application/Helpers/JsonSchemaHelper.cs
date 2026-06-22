using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Amigo.Application.Helpers
{
    public static class JsonSchemaHelper
    {
        public static string GenerateToursTranslationSchema()
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

                    "destination": {
                      "destinationId": "guid",
                      "name": "string"
                    },

                    "inclusions": [
                      {
                        "inclusionId": "guid",
                        "text": "string"
                      }
                    ],

                    "prices": [
                      {
                        "priceId": "guid",
                        "type": "string",
                        "ActivityType": "string"
                      }
                    ]
                  }
                ]
              }
            ]
            """;
        }

        public static string GenerateDestinationTranslationSchema()
        {
             return """
            [
              {
                "language": "string",
                "destination": {
                  "destinationId":  "guid",
                  "name": "string",
                  "description": "string,
                  "countryDescription: "string"
                }
              }
            ]
            """;
        }
        public static string GenerateTourTranslationSchema()
        {
            return """
            [
              {
                "language": "string",
                "tour": 
                  {
                    "tourId": "guid",
                    "title": "string",
                    "description": "string",

                    "inclusions": [
                      {
                        "inclusionId": "guid",
                        "text": "string"
                      }
                    ],

                    "prices": [
                      {
                        "priceId": "guid",
                        "type": "string",
                        "ActivityType": "string"
                      }
                    ]
                  }
                
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

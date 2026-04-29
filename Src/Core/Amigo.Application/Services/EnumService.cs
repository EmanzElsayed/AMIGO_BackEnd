

using Amigo.Domain.DTO.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Amigo.Application.Services
{
    public class EnumService : IEnumService
    {
        Dictionary<Language,string> GetLanguageName = new Dictionary<Language, string>() {

            { Language.en , "English" },
            { Language.it , "Italiano" },
            { Language.fr , "Français" },
            { Language.br , "Português (BR)" },
            { Language.pt , "Português (PT)" },
            { Language.es , "Español" },


        };

        public Result<List<GetLanguageResponseDTO>> GetLanguageEnum()
        {
            
            var result =  Enum.GetValues(typeof(Language))
                .Cast<Language>()
                .Where(e => Convert.ToInt32(e) != 0)
                .Select(e => new GetLanguageResponseDTO(
                        LanguageCode :e.ToString(),
                        Name : GetLanguageName[e]

                    )
                ).ToList();

            return Result.Ok(result)
                   .WithSuccess(new Success("Get Enum Successfully!!"));


        }

        public Result<IEnumerable< GetEnumResponseDTO>> GetEnum<T>() where T : Enum
        {
            var result =  Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .Where(e => Convert.ToInt32(e) != 0)
                    .Select(e => new GetEnumResponseDTO(

                        Id: Convert.ToInt32(e)
                        , Name: GetDisplayName(e)


                    )

            );
            
            return Result.Ok( result)
                         .WithSuccess(new Success("Get Enum Successfully!!"));



        }

        private static string GetDisplayName(Enum value)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();

            var displayAttr = member?.GetCustomAttribute<DisplayAttribute>();

            return displayAttr?.Name ?? value.ToString();
        }



    }
}

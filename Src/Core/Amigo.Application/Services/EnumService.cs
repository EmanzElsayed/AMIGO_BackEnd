

using Amigo.Domain.DTO.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Amigo.Application.Services
{
    public class EnumService(IConfiguration _configuration) : IEnumService
    {
        Dictionary<SupportedLanguage,string> GetLanguageName = new Dictionary<SupportedLanguage, string>() {

            { SupportedLanguage.en , "English" },
            { SupportedLanguage.it , "Italiano" },
            { SupportedLanguage.fr , "Français" },
            { SupportedLanguage.br , "Português (BR)" },
            { SupportedLanguage.pt , "Português (PT)" },
            { SupportedLanguage.es , "Español" },


        };

        public Result<List<GetLanguageResponseDTO>> GetLanguageEnum()
        {
            
            var result =  Enum.GetValues(typeof(SupportedLanguage))
                .Cast<SupportedLanguage>()
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

        public Result<ContactInfoDTO> GetContactInfo()
        {
            return new ContactInfoDTO(
                    WebsiteLink: _configuration["ContactInfo:WebsiteLink"],
                    PhoneNumber: _configuration["ContactInfo:PhoneNumber"],
                    FacebookLink: _configuration["ContactInfo:FacebookLink"],
                    YoutubeLink: _configuration["ContactInfo:YoutubeLink"],
                    InstaLink: _configuration["ContactInfo:InstaLink"],
                    LinkedInLink: _configuration["ContactInfo:LinkedInLink"],
                    CreatedAt: _configuration["ContactInfo:CreatedAt"],
                    Email: _configuration["ContactInfo:Email"],
                    Location: _configuration["ContactInfo:Location"]


                );
        }
    }
}

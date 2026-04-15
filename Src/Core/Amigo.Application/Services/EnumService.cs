

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Amigo.Application.Services
{
    public class EnumService : IEnumService
    {
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
